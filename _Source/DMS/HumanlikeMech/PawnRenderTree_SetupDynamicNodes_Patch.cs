using Verse;
using HarmonyLib;
using System;

namespace DMS
{
    [HarmonyPatch(typeof(PawnRenderTree), "SetupDynamicNodes")]
    public class PawnRenderTree_SetupDynamicNodes_Patch
    {
        private static void Postfix(PawnRenderTree __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn is HumanlikeMech)
            {
                Traverse.Create((object)__instance).Method("SetupApparelNodes", Array.Empty<object>()).GetValue();
            }
        }
    }
}
