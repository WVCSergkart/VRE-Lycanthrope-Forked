﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaRacesExpandedLycanthrope
{
    [DefOf]
    public static class InternalDefOf
    {

     

        //public static MorphGeneTemplateDef VRE_Morphs;

        //public static AbilityDef VRE_Morph;

        //public static GeneDef VRE_Morphs_NocturnalMorphing;
        //public static GeneDef VRE_Morphs_AdulthoodMorphing;
        //public static GeneDef VRE_Morphs_SeasonalMorphing;
        //public static GeneDef VRE_Morphs_DamageMorphing;
        //public static GeneDef VRE_Morphs_RandomMorphing;
        public static GeneDef VRE_Photophobia;
        public static GeneDef VRE_PackMentality;
        //public static GeneDef VRE_GermlineReimplanter;

        public static PawnRelationDef VRE_PackMember;

        public static HediffDef VRE_WarHowlHediff;
        //public static HediffDef VRE_EndogerminationComa;
        //public static HediffDef VRE_EndogermLossShock;

        public static EffecterDef CocoonDestroyed;

        public static SoundDef VRE_WarHowl_Cast;
        public static SoundDef VRE_MorphSound;

        public static FactionDef VRE_WolfmenFaction;

        public static PawnKindDef VRE_WolfmenPawnKindDef;

        //public static JobDef VRE_AbsorbGermline;

        static InternalDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
        }

       


    }
}