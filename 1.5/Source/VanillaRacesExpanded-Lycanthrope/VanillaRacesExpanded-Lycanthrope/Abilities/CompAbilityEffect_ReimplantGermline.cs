﻿
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace VanillaRacesExpandedLycanthrope
{
    public class CompAbilityEffect_ReimplantGermline : CompAbilityEffect
    {
        private static readonly CachedTexture ReimplantIcon = new CachedTexture("UI/Icons/Genes/Gene_GermlineReimplanter");

        private new CompProperties_AbilityReimplantGermline Props => (CompProperties_AbilityReimplantGermline)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {

            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn != null)
            {
                ReimplantGermline(parent.pawn, pawn);
                FleckMaker.AttachedOverlay(pawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
                if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    int max = InternalDefOf.VRE_EndogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
                    int max2 = InternalDefOf.VRE_EndogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
                    Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "VRE_LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, pawn));
                }
            }
        }

        public static void ReimplantGermline(Pawn caster, Pawn recipient)
        {

            QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));

            List<GeneDef> xenogenesToPreserve = new List<GeneDef>(); 
            foreach(Gene gene in recipient.genes.Xenogenes)
            {
                xenogenesToPreserve.Add(gene.def);
            }
            
            recipient.genes.SetXenotype(caster.genes.Xenotype);
            recipient.genes.xenotypeName = caster.genes.xenotypeName;
            recipient.genes.xenotypeName = caster.genes.xenotypeName;
            recipient.genes.iconDef = caster.genes.iconDef;
            if (recipient.genes?.Endogenes?.Count > 0)
            {
                for (int num = recipient.genes.Endogenes.Count - 1; num >= 0; num--)
                {
                    recipient.genes.RemoveGene(recipient.genes.Endogenes[num]);
                }
            }
            List<Gene> adjustedList = new List<Gene>();

            foreach (Gene endogene in caster.genes.Endogenes)
            {
                if (endogene.def.defName.Contains("VRE_Morphs"))
                {
                    if (endogene.Active)
                    {
                        adjustedList.Add(endogene);
                    }
                }else adjustedList.Add(endogene);
            }


            foreach (Gene endogene in adjustedList)
            {
                recipient.genes.AddGene(endogene.def, xenogene: false);
                
            }

            foreach (GeneDef xenogene in xenogenesToPreserve)
            {
              

                recipient.genes.AddGene(xenogene, xenogene: true);

            }
            if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
            {
                caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
            }
            recipient.health.AddHediff(InternalDefOf.VRE_EndogerminationComa);
            ExtractGermline(caster);
            GeneUtility.UpdateXenogermReplication(recipient);

        }

        public static void ExtractGermline(Pawn pawn, int overrideDurationTicks = -1)
        {
            pawn.health.AddHediff(InternalDefOf.VRE_EndogermLossShock);
            if (GeneUtility.PawnWouldDieFromReimplanting(pawn))
            {
                pawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
            }
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.XenogermReplicating, pawn);
            if (overrideDurationTicks > 0)
            {
                hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
            }
            pawn.health.AddHediff(hediff);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return base.Valid(target, throwMessages);
            }
            if (pawn.IsQuestLodger())
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (!parent.pawn.genes.Endogenes.Any())
            {
                if (throwMessages)
                {
                    Messages.Message("VRE_MessagePawnHasNoGermline".Translate(parent.pawn), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            
            if (!PawnIdeoCanAcceptReimplant(parent.pawn, pawn))
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }

        public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
        {
            if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
            {
                return Dialog_MessageBox.CreateConfirmation("VRE_WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
            }
            return null;
        }

        public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
        {
            Pawn pawn = target.Pawn;
            yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (CanAbsorbGermline(parent.pawn))
            {
                Pawn myPawn = parent.pawn;
                Command_Action command_Action = new Command_Action
                {
                    defaultLabel = "VRE_ForceGermlineImplantation".Translate(),
                    defaultDesc = "VRE_ForceGermlineImplantationDesc".Translate(),
                    icon = ReimplantIcon.Texture,
                    action = delegate
                    {
                        List<FloatMenuOption> list = new List<FloatMenuOption>();
                        List<Pawn> list2 = myPawn.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
                       
                        for (int i = 0; i < list2.Count; i++)
                        {
                            Pawn absorber = list2[i];
                            if (absorber.genes != null && absorber.IsColonistPlayerControlled && !GeneUtility.SameXenotype(absorber, myPawn) && absorber.CanReach(myPawn, PathEndMode.ClosestTouch, Danger.Deadly))
                            {
                                if (!PawnIdeoCanAcceptReimplant(parent.pawn, absorber))
                                {
                                    list.Add(new FloatMenuOption(absorber.LabelCap + ": " + "IdeoligionForbids".Translate(), null, absorber, Color.white));
                                }
                                else
                                {
                                    list.Add(new FloatMenuOption(absorber.LabelShort, delegate
                                    {
                                        if (GeneUtility.PawnWouldDieFromReimplanting(myPawn))
                                        {
                                            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("VRE_WarningPawnWillDieFromReimplanting".Translate(myPawn.Named("PAWN")), delegate
                                            {
                                                
                                                Log.Message("Calling GiveReimplantJob with " + absorber);
                                                GiveReimplantJob(absorber, myPawn);
                                            }, destructive: true));
                                        }
                                        else
                                        {
                                            Log.Message("Calling GiveReimplantJob with " + absorber);
                                            GiveReimplantJob(absorber, myPawn);
                                        }
                                    }, absorber, Color.white));
                                }
                            }
                        }
                        if (list.Any())
                        {
                            Find.WindowStack.Add(new FloatMenu(list));
                        }
                    }
                };
                if (myPawn.IsQuestLodger())
                {
                    command_Action.Disable("TemporaryFactionMember".Translate(myPawn.Named("PAWN")));
                }
                else if (myPawn.health.hediffSet.HasHediff(InternalDefOf.VRE_EndogermLossShock))
                {
                    command_Action.Disable("VRE_EndogeneLossShockPresent".Translate(myPawn.Named("PAWN")));
                }
                else if (myPawn.IsPrisonerOfColony && !myPawn.Downed)
                {
                    command_Action.Disable("VRE_MessageTargetMustBeDownedToForceReimplant".Translate(myPawn.Named("PAWN")));
                }
                yield return command_Action;
            }
        }

        public static void GiveReimplantJob(Pawn pawn, Pawn targPawn)
        {
            Log.Message("Assigning job to extract from " + targPawn + " to " + pawn);
            pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.VRE_AbsorbGermline, targPawn), JobTag.Misc);
            if (targPawn.HomeFaction != null && !targPawn.HomeFaction.Hidden && targPawn.HomeFaction != pawn.Faction && !targPawn.HomeFaction.HostileTo(Faction.OfPlayer))
            {
                Messages.Message("VRE_MessageAbsorbingGermlineWillAngerFaction".Translate(targPawn.HomeFaction, targPawn.Named("PAWN")), pawn, MessageTypeDefOf.CautionInput, historical: false);
            }
        }


        public static bool CanAbsorbGermline(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return false;
            }
            if (!pawn.genes.HasActiveGene(InternalDefOf.VRE_GermlineReimplanter))
            {
                return false;
            }
            if (pawn.IsPrisonerOfColony && pawn.guest.PrisonerIsSecure)
            {
                return true;
            }
            if (!pawn.Downed)
            {
                return false;
            }
            if (!pawn.genes.Endogenes.Any())
            {
                return false;
            }
            return true;
        }

        public static bool PawnIdeoCanAcceptReimplant(Pawn implanter, Pawn implantee)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return true;
            }
           
            if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.BecomeNonPreferredXenotype, implantee) && !implantee.Ideo.IsPreferredXenotype(implanter))
            {
                return false;
            }
            return true;
        }
    }
}