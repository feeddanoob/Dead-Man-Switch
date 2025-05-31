using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static RimWorld.MechClusterSketch;

namespace DMS
{
    [HarmonyPatch(typeof(CompOverseerSubject), "State",MethodType.Getter)]
    public static class Patch_Overseer
    {
        [HarmonyPostfix]
        public static void Postfix(ref OverseerSubjectState __result, CompOverseerSubject __instance)
        {
            if (__instance.parent.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch comp && comp.woken)
            {
                __result = OverseerSubjectState.Overseen;
            }

            //甚至不確定有沒有效。
            else if (__instance.parent is Pawn p && p.HostFaction == Faction.OfPlayer)
            {
                __result = OverseerSubjectState.Overseen;
            }
        }
    }

    [HarmonyPatch(typeof(MechanitorUtility), "CanDraftMech")]
    public static class Patch_MechanitorDraft
    {
        [HarmonyPostfix]
        public static void Postfix(ref AcceptanceReport __result, Pawn mech)
        {
            if (mech.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch comp && comp.woken)
            {
                __result = true;
            }

            //甚至不確定有沒有效。
            else if (mech.HostFaction == Faction.OfPlayer)
            {
                __result = true;
            }
        }
    }
    [HarmonyPatch(typeof(Pawn_DraftController), "ShowDraftGizmo",MethodType.Getter)]
    public static class Patch_MechDraft
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn_DraftController __instance)
        {
            if (__instance.pawn.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch comp && comp.woken)
            {
                __result = true;
            }

            //甚至不確定有沒有效。
            else if (__instance.pawn.HostFaction == Faction.OfPlayer)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(PawnComponentsUtility), "AddComponentsForSpawn")]
    public static class Patch_MechInteracte
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            if (pawn.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch comp && comp.woken)
            {
                if (pawn.interactions == null)
                {
                    pawn.interactions = new Pawn_InteractionsTracker(pawn);
                }
            }
        }
    }
}
