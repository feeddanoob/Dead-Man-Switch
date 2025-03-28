using Verse;
using RimWorld;
using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(Building_TurretGun), "CanSetForcedTarget", (MethodType)1)]
    internal static class Patch_ForceTarget
    {
        public static void Postfix(ref bool __result, Building_TurretGun __instance)
        {
            if (__instance.TryGetComp<CompForceTargetable>() != null && __instance.Faction != null && __instance.Faction.IsPlayer)
            {
                __result = true;
            }
        }
    }
}