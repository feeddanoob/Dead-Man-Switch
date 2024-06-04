using Verse;
using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker_Apparel_Body), nameof(PawnRenderNodeWorker_Apparel_Body.CanDrawNow))]
    public static class Patch_PawnRenderNodeWorker_Apparel_Body_CanDrawNow
    {
        public static bool Prefix(PawnRenderNode node, PawnDrawParms parms, ref bool __result)
        {
            if (parms.pawn is HumanlikeMech)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
