using RimWorld;
using Verse;

namespace DMS
{
    //在失去機械師控制後只會自動關機
    public class CompDeadManSwitch : ThingComp
    {
        public CompProperties_DeadManSwitch Props => (CompProperties_DeadManSwitch)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
        }
        public override string CompInspectStringExtra()
        {   if (parent.Faction != Faction.OfPlayer || parent.GetComp<CompOverseerSubject>() == null) return null;
            if (parent.GetComp<CompOverseerSubject>().State != OverseerSubjectState.Overseen)
            {
                string str = "DMS_WillTerminateTheBetrayedUnit".Translate();
                return str;
            }
            return null;
        }
    }

    public class CompProperties_DeadManSwitch : CompProperties
    {
        public int minDelayUntilDMS = 3000;

        public CompProperties_DeadManSwitch()
        {
            compClass = typeof(CompDeadManSwitch);
        }
    }
}
