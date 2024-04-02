using Verse;
using RimWorld;
using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "CanControlColonist", MethodType.Getter)]
    public static class Patch_CanDropWeapon
    {
        [HarmonyPostfix]
        public static void CanControl(ref bool __result)
        {
            if (__result) return;
            Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn != null)
            {
                if (pawn is IWeaponUsable && pawn.Faction.IsPlayer)
                {
                    __result = true;
                }
            }
        }
    }
}