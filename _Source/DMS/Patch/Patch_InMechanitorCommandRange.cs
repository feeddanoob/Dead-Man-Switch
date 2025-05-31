﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DMS
{
    [HarmonyPatch(typeof(MechanitorUtility), "InMechanitorCommandRange")]
    internal class Patch_InMechanitorCommandRange
    {
        private static void Postfix(Pawn mech, LocalTargetInfo target, ref bool __result)
        {
            if (__result) return;
            if (mech.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch compDMS && compDMS.woken)
            {
                __result = true;
                return;
            }
            if (mech.TryGetComp<CompCommandRelay>(out var _c))
            {
                __result = true;
                return;
            }

            List<Pawn> overseenPawns = MechanitorUtility.GetOverseer(mech)?.mechanitor?.OverseenPawns;
            if (overseenPawns.NullOrEmpty()) return;
            foreach (Pawn item in overseenPawns.Where(p => p.Spawned && p.MapHeld == mech.MapHeld))
            {
                if (item.TryGetComp<CompCommandRelay>(out var c) && CheckUtility.InRange(item, target, c.SquaredDistance))
                {
                    __result = true;
                    return;
                }
            }

            foreach (Pawn p in mech.Map.mapPawns.SpawnedPawnsInFaction(RimWorld.Faction.OfPlayer))
            {
                if (CheckUtility.HasSubRelay(p, out CompSubRelay sub) && CheckUtility.InRange(p, target, sub.SquaredDistance))
                {
                    __result = true;
                    return;
                }
            }
        }
    }
}