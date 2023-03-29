using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;

using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class Patch_CanTakeOrder
    {
        [HarmonyPostfix]
        public static void AllowTakeOrder(Pawn pawn,ref bool __result)
        {
            if (pawn is HumanlikeMech)
            {
                __result = true;
            }
        }
    }
}
