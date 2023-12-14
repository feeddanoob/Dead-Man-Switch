using RimWorld;
using System.Collections.Generic;
using Verse;
namespace DMS
{
    [DefOf]
    public static class DMS_ThingDefOf
    {
        public static ThingDef DMS_BandNode;
    }
    public class Hediff_SeverityByBandwidth : HediffWithComps
    {
        private int cachedTunedBandNodesCount;

        private HediffStage curStage;

        public int AdditionalBandwidth => cachedTunedBandNodesCount;

        public override HediffStage CurStage
        {
            get
            {
                if (curStage == null && cachedTunedBandNodesCount > 0)
                {
                    StatModifier statModifier = new StatModifier();
                    statModifier.stat = StatDefOf.MechBandwidth;
                    statModifier.value = cachedTunedBandNodesCount*4;
                    curStage = new HediffStage();
                    curStage.statOffsets = new List<StatModifier> { statModifier };
                }

                return curStage;
            }
        }
        public override void PostTick()
        {
            base.PostTick();
            if (pawn.IsHashIntervalTick(60))
            {
                if (pawn.mechanitor != null)
                {
                    StatModifier statModifier = new StatModifier();
                    statModifier.stat = StatDefOf.MechBandwidth;
                    this.Severity = pawn.mechanitor.UsedBandwidth != 0 ? (float)pawn.mechanitor.UsedBandwidth / (float)pawn.mechanitor.TotalBandwidth : 0.1f;
                }
                RecacheBandNodes();
            }
        }
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            RecacheBandNodes();
        }
        public void RecacheBandNodes()
        {
            int num = cachedTunedBandNodesCount;
            cachedTunedBandNodesCount = 0;
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                foreach (Building item in maps[i].listerBuildings.AllBuildingsColonistOfDef(DMS_ThingDefOf.DMS_BandNode))
                {
                    if (item.TryGetComp<CompBandNode>().tunedTo == pawn && item.TryGetComp<CompPowerTrader>().PowerOn)
                    {
                        cachedTunedBandNodesCount++;
                    }
                }
            }

            if (num != cachedTunedBandNodesCount)
            {
                curStage = null;
                pawn.mechanitor?.Notify_BandwidthChanged();
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cachedTunedBandNodesCount, "cachedTunedBandNodesCount", 0);
        }
    }
}
