using RimWorld;
using Verse;
namespace DMS
{
    public class HediffComp_SeverityByBandwidth : HediffComp
    {
        public override void CompPostMake()
        {
            base.CompPostMake();
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            parent.Severity = Pawn.mechanitor.UsedBandwidth != 0 ? (float)Pawn.mechanitor.UsedBandwidth / (float)Pawn.mechanitor.TotalBandwidth : 0.1f;
        }
    }
}
