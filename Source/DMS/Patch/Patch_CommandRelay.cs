using HarmonyLib;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    [HarmonyPatch(typeof(MechanitorUtility), "InMechanitorCommandRange")]
    internal class Patch
    {
        // Token: 0x06000006 RID: 6 RVA: 0x000020F0 File Offset: 0x000002F0
        private static void Postfix(Pawn mech, LocalTargetInfo target, ref bool __result)
        {
            bool isOutOfDeafultRange = !__result;
            if (isOutOfDeafultRange)
            {
                if (mech.kindDef.race.HasComp(typeof(CompCommandRelay)))
                {
                    __result = true;
                }
                List<Pawn> overseenPawns = MechanitorUtility.GetOverseer(mech).mechanitor.OverseenPawns;
                foreach (Pawn item in overseenPawns)
                {
                    if (item.GetComp<CompCommandRelay>() != null && item.Drafted)
                    {
                        if (target.ToGlobalTargetInfo(Find.CurrentMap).Map == item.MapHeld && (float)IntVec3Utility.DistanceTo(item.Position, target.Cell) < mech.GetComp<CompCommandRelay>().currentRadius)
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
