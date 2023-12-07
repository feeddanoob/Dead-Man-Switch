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
        public override void CompTickRare()
        {
            //if (parent.Faction != Faction.OfPlayer) return;

            //Pawn p = parent as Pawn;
            //if (!p.Dead)
            //{
            //    if (parent.GetComp<CompOverseerSubject>().DelayUntilFeralCheckTicks <= Props.minDelayUntilDMS)
            //    {
            //        parent.Kill();
            //    }
            //}
        }
        public override string CompInspectStringExtra()
        {   if (parent.Faction != Faction.OfPlayer || parent.GetComp<CompOverseerSubject>() == null) return null;
            if (parent.GetComp<CompOverseerSubject>().State != OverseerSubjectState.Overseen)
            {
                string str = "DMS will terminate the betrayed unit.".Translate();
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
