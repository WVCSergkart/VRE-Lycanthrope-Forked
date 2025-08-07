using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaRacesExpandedLycanthrope
{

    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
    public static class Patch_BackCompatibility_BackCompatibleDefName
    {
        public static void Postfix(Type defType, string defName, ref string __result)
        {
            if (typeof(GeneDef).IsAssignableFrom(defType))
            {
                if (defName == "VRE_SpeechInability")
                {
                    __result = "WVC_AgeDebuff_Social";
                }
                //if (defName == "VRE_SpeechInability")
                //{
                //    __result = "WVC_AgeDebuff_Social";
                //}
                //if (defName == "VRE_SpeechInability")
                //{
                //    __result = "WVC_AgeDebuff_Social";
                //}
                //if (defName == "VRE_SpeechInability")
                //{
                //    __result = "WVC_AgeDebuff_Social";
                //}
                //if (defName == "VRE_SpeechInability")
                //{
                //    __result = "WVC_AgeDebuff_Social";
                //}
                //if (defName == "VRE_SpeechInability")
                //{
                //    __result = "WVC_AgeDebuff_Social";
                //}
            }
        }
    }

}
