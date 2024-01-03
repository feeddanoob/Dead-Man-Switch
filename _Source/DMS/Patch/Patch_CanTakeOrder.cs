using System;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class Patch_CanTakeOrder
    {
        [HarmonyPostfix]
        public static void AllowTakeOrder(Pawn pawn,ref bool __result)
        {
            if (pawn is HumanlikeMech | pawn is WeaponUsableMech | pawn is WeaponUsableMachine)
            {
                __result = true;
            }
        }
    }
}
