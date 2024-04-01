using Verse;
using RimWorld;

using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "CanControl", MethodType.Getter)]
    public static class Patch_CanDropWeapon
    {
        [HarmonyPrefix]
        public static bool CanControl(ref bool __result)
        {
            if (__result) return true;
            if (Find.Selector.SingleSelectedThing is Pawn selectedPawn)
            {
                if (selectedPawn is IWeaponUsable && selectedPawn.Faction.IsPlayer)
                {
                    return true;
                }
            }
            return __result;
        }
    }
}