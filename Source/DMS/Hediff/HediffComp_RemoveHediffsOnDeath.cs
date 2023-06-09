using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class HediffCompProperties_RemoveHediffsOnDeath : HediffCompProperties
    {
        public HediffCompProperties_RemoveHediffsOnDeath()
        {
            compClass = typeof(HediffComp_RemoveHediffsOnDeath);
        }
        public List<HediffDefWtihSeverity> hediffs;
    }
    //在死亡後添加或移除Hediff
    public class HediffComp_RemoveHediffsOnDeath : HediffComp
    {
        private HediffCompProperties_RemoveHediffsOnDeath Props
        {
            get
            {
                return (HediffCompProperties_RemoveHediffsOnDeath)this.props;
            }
        }
        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            OnDeathHediffOperation(base.Pawn);
        }
        private void OnDeathHediffOperation(Pawn pawn)
        {
            for (int i = 0; i < this.Props.hediffs.Count; i++)
            {
                if (pawn.health.hediffSet.HasHediff(Props.hediffs[i].hediffDef))
                {
                    pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffs[i].hediffDef));
                }
            }
        }
    }
}
