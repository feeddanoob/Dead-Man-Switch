using Verse;
using RimWorld;

using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(Building_TurretGun), "CanSetForcedTarget",  (MethodType)1)]
    internal static class Patch_ForceTarget
    {
        public static void Postfix(ref bool __result, Building_TurretGun __instance)
        {
            if (ThingCompUtility.TryGetComp<CompForceTargetable>(__instance) != null)
            {
                __result = true;
            }
        }
    }
}
