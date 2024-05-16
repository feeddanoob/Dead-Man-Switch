using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class CompTargetable_TargetOnMech : CompTargetable
    {
            protected override bool PlayerChoosesTarget => true;

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetMechs = true,
                onlyRepairableMechs = true,
            };
        }
    }
}
