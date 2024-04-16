using Verse;
using RimWorld;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace DMS
{
    [HarmonyPatch(typeof(ImmunityHandler),
        nameof(ImmunityHandler.DiseaseContractChanceFactor),
        new Type[] { typeof(HediffDef), typeof(HediffDef), typeof(BodyPartRecord) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    internal static class DiseaseContractChanceFactor
    {
        public static void Postfix(ImmunityHandler __instance, ref float __result)
        {
            if (__result == 0f) return;
            if (__instance.pawn != null && __instance.pawn.apparel != null && __instance.pawn.apparel.AnyApparel)
            {
                foreach (var item in __instance.pawn.apparel.WornApparel)
                {
                    if (item.def.HasModExtension<BiochemicalProtectionExtension>()) __result = 0f;
                }
            }
        }
    }
}
