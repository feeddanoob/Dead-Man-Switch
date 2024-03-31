using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace DMS
{
    [HarmonyPatch(typeof(Pawn_MechanitorTracker), nameof(Pawn_MechanitorTracker.CanControlMechs),MethodType.Getter)]
    internal static class Patch_CanControlMechs
    {
        static void Postfix(Pawn_MechanitorTracker __instance, ref AcceptanceReport __result)
        {
            if (__result == true) return;
            if (__instance.OverseenPawns?.Where(p => p.TryGetComp<CompCommandRelay>() != null)?.Count() > 0) __result = true;
        }
    }

    [HarmonyPatch(typeof(MechanitorUtility), nameof(MechanitorUtility.CanDraftMech), MethodType.Normal)]
    internal static class Patch_CanDraftMech
    {
        static void Postfix(Pawn mech, ref AcceptanceReport __result)
        {
            if (__result == true || !mech.IsColonyMech || mech.DeadOrDowned) return;

            if (mech.kindDef.race.HasComp(typeof(CompCommandRelay)))
            {
                //Log.Message("IsCommandRelay");
                __result = true;
                return;
            }
            List<Pawn> overseenPawns = MechanitorUtility.GetOverseer(mech)?.mechanitor?.OverseenPawns;
            List<Pawn> commandRelay = overseenPawns.Where(temp => temp.TryGetComp<CompCommandRelay>() != null).ToList();
            if (commandRelay.Count != 0)
            {
                foreach (Pawn pawn in overseenPawns.Where(p => p.MapHeld == mech.MapHeld))
                {
                    if (commandRelay.Contains(pawn))
                    {
                        // Log.Message("SameMapAsCommandRelay");
                        __result = true;
                        return;
                    }
                }
            }
            __result = false;
        }
    }
}