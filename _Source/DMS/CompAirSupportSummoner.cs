using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DMS
{
    public class CompAirSupportSummoner : ThingComp
    {
        CompProperties_AirSupportSummoner Props => props as CompProperties_AirSupportSummoner;
        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            yield return new Command_Action
            {
                action = new Action(DoEffect),
                defaultDesc = parent.def.description,
                icon = parent.def.uiIcon,
                defaultLabel = "boom",
            };
        }

        public void DoEffect()
        {
            Targeter targeter = Find.Targeter;
            var pram = new TargetingParameters
            {
                canTargetBuildings = false,
                canTargetPawns = false,
                canTargetLocations = true
            };
            Find.Targeter.BeginTargeting(pram, DoEffect);
        }

        public void DoEffect(LocalTargetInfo cell)
        {
            int delay = Find.TickManager.TicksGame + Props.delayRange.RandomInRange;
            var c = GenRadial.NumCellsInRadius(Props.spreadRadius);
            var ori = CellFinder.RandomEdgeCell(parent.MapHeld).ToVector3Shifted();

            Thing triggerer = parent.ParentHolder is Pawn_ApparelTracker pawn ? pawn.pawn : parent;

            for (int i = 0; i < Props.burstCount; i++)
            {
                GameComponent_CAS.AddData(new AirSupportData_LaunchProjectile()
                {
                    projectileDef = Props.ProjectileDef,
                    map = parent.MapHeld,
                    targetCell = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + cell.Cell,
                    triggerTick = delay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                    origin = ori
                });
                delay += Props.burstInterval;
            }
        }
    }

    public class CompProperties_AirSupportSummoner : CompProperties
    {
        public CompProperties_AirSupportSummoner()
        {
            compClass = typeof(CompAirSupportSummoner);
        }

        public ThingDef ProjectileDef;

        public float spreadRadius = 5;

        public int burstCount = 1, burstInterval = 5;

        public IntRange delayRange = new IntRange(120, 150);
    }
}
