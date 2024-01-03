using HarmonyLib;
using RimWorld;
using Verse;

namespace DMS
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(Pawn), "DropAndForbidEverything")]
    static class Patch_Pawn
    {
        [HarmonyPrefix]
        static bool PreFix(Pawn __instance)
        {
            if (__instance is WeaponUsable && __instance.Faction != Faction.OfPlayer)
            {
                __instance.equipment.DestroyAllEquipment();
                __instance.apparel.DestroyAll();
            }
            return true;
        }
    }
}
