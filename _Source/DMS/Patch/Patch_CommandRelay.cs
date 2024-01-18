using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    [HarmonyPatch(typeof(MechanitorUtility), "InMechanitorCommandRange")]
    internal class Patch_CommandRelay
    {
        private static void Postfix(Pawn mech, LocalTargetInfo target, ref bool __result)
        {
            bool isOutOfDeafultRange = !__result;
            if (isOutOfDeafultRange)
            {
                if (mech.kindDef.race.HasComp(typeof(CompCommandRelay)))
                {
                    __result = true;
                    return;
                }
                List<Pawn> overseenPawns = MechanitorUtility.GetOverseer(mech)?.mechanitor?.OverseenPawns;
                foreach (Pawn item in overseenPawns)
                {
                    if (item.GetComp<CompCommandRelay>() != null && item.Drafted && item.Map == mech.Map)
                    {
                        if ((float)IntVec3Utility.DistanceTo(item.Position, target.Cell) < item.GetComp<CompCommandRelay>().currentRadius)
                        {
                            __result = true;
                        }
                    }
                }
            }
        }
    }
}
