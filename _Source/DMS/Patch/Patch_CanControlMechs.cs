using Verse;
using HarmonyLib;
using RimWorld;
using System.Linq;

namespace DMS
{
    [HarmonyPatch(typeof(Pawn_MechanitorTracker), nameof(Pawn_MechanitorTracker.CanControlMechs), MethodType.Getter)]
    internal static class Patch_CanControlMechs
    {
        internal static void Postfix(Pawn_MechanitorTracker __instance, ref AcceptanceReport __result)
        {
            if (__result == true) return;
            if (__instance.Pawn.HostFaction != null) __result = true;
            if (__instance.OverseenPawns?.Where(p => p.TryGetComp<CompCommandRelay>() != null)?.Count() > 0) __result = true;
        }
    }
}