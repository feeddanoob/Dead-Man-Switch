using Verse;
using RimWorld;
using System.Reflection;
using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(MetalhorrorUtility), nameof(MetalhorrorUtility.CanBeInfected))]
    internal static class Patch_CanBeInfected
    {
        public static void Postfix(ref bool __result, Pawn pawn)
        {
            if (__result)
            {
                if (pawn.equipment.HasAnything())
                {
                    foreach (ThingWithComps apparel in pawn.equipment.AllEquipmentListForReading)
                    {
                        if (apparel.def.GetModExtension<BiochemicalProtectionExtension>() != null) __result = false;
                    }
                }
            }
        }
    }
}
