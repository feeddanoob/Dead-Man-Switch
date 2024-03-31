using Verse;
using RimWorld;

using HarmonyLib;

namespace DMS
{

    [HarmonyPatch(typeof(FloatMenuMakerMap),"CanTakeOrder")]
    public static class Patch_CanTakeOrder
    {
        [HarmonyPostfix]
        public static void AllowTakeOrder(Pawn pawn, ref bool __result)
        {
            if (pawn is IWeaponUsable && !pawn.DeadOrDowned)
            {
                __result = true;
            }
        }
    }
}