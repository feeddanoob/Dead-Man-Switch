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
        public static void Postfix(ImmunityHandler __instance, float __result)
        {
            Log.Message(__instance.pawn.Name.ToString());
            if (__instance.pawn != null && __instance.pawn.equipment != null && __instance.pawn.equipment.HasAnything())
            {
                foreach (var item in __instance.pawn.equipment.AllEquipmentListForReading)
                {
                    if (item.def.HasModExtension<BiochemicalProtectionExtension>())
                    {
                        __result = 0f;
                    }
                }
            }
        }
    }
}
