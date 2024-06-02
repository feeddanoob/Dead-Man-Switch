using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace DMS
{
    public class CompTargetable_AddHediffOnTarget : CompTargetable
    {
        public new CompProperties_AddHediffOnTarget Props => (CompProperties_AddHediffOnTarget)props;
        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetHumans = false,
                canTargetMutants = false,
                canTargetAnimals = false,
                canTargetMechs = true,
                canTargetBuildings = false,
                canTargetLocations = false
            };
        }
        protected override bool PlayerChoosesTarget => true;
        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }
        public override void DoEffect(Pawn usedBy)
        {

            if (this.PlayerChoosesTarget && this.selectedTarget == null)
            {
                return;
            }
            if (this.selectedTarget != null && !this.GetTargetingParameters().CanTarget(this.selectedTarget, null))
            {
                return;
            }
            if (usedBy.IsColonistPlayerControlled)
            {
                Job job = JobMaker.MakeJob(DMS_JobDefOf.DMS_Modification, (Pawn)selectedTarget, this.parent);
                job.count = 1;
                job.playerForced = true;
                usedBy.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }

    }
    public class CompProperties_AddHediffOnTarget : CompProperties_Targetable
    {
        public HediffDef hediffDef;
        public SoundDef soundDef;
    }
}
