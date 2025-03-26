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

            Thing triggerer = parent.ParentHolder is Pawn_ApparelTracker pawn ? pawn.pawn : parent;
            var ori = CellFinder.RandomEdgeCell(parent.MapHeld).ToVector3Shifted();//改這裡到時候換成最近的砲兵設施或殖民艦隊基地。
            int delay = Find.TickManager.TicksGame + Props.supportDef.triggerTick.RandomInRange;
            var c = GenRadial.NumCellsInRadius(Props.supportDef.spreadRadius.RandomInRange);
            for (int i = 0; i < Props.supportDef.burstCount; i++)
            {
                GameComponent_CAS.AddData(new AirSupportData_LaunchProjectile()
                {
                    projectileDef = Props.supportDef.projectileDef,
                    map = parent.MapHeld,
                    targetCell = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + cell.Cell,
                    triggerTick = delay,
                    def = Props.supportDef,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                    origin = ori
                });
                delay += Props.supportDef.burstInterval;
            }

            if (!triggerer.Faction.HostileTo(Faction.OfPlayer) && !Props.supportDef.preMessage.NullOrEmpty())
            {
                Messages.Message(Props.supportDef.preMessage.Translate(triggerer.Faction.Named("FACTION"), Props.supportDef.LabelCap, Props.supportDef.triggerTick.RandomInRange.TicksToSeconds()), new LookTargets(triggerer.PositionHeld, triggerer.MapHeld), MessageTypeDefOf.NeutralEvent);
            }
        }
    }
    public class CompProperties_AirSupportSummoner : CompProperties
    {
        public CompProperties_AirSupportSummoner()
        {
            compClass = typeof(CompAirSupportSummoner);
        }
        public SupportDef supportDef;
    }
}
