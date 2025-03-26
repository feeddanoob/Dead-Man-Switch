using Mono.Unix.Native;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class RoyalTitlePermitWorker_AirSupport : RoyalTitlePermitWorker_Targeted
    {
        private Faction faction;
        public override IEnumerable<FloatMenuOption> GetRoyalAidOptions(Map map, Pawn pawn, Faction faction)
        {
            if (faction.HostileTo(Faction.OfPlayer))
            {
                yield return new FloatMenuOption("CommandCallRoyalAidFactionHostile".Translate(faction.Named("FACTION")), null);
                yield break;
            }
            Action action = null;
            string description = def.LabelCap + ": ";
            if (FillAidOption(pawn, faction, ref description, out var free))
            {
                action = new Action(DoEffect);
            }
            yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
        }
        private void BeginCall(Pawn caller, Faction faction, Map map, bool free)
        {
            targetingParameters = new TargetingParameters();
            targetingParameters.canTargetLocations = true;
            targetingParameters.canTargetBuildings = false;
            targetingParameters.canTargetPawns = false;
            base.caller = caller;
            base.map = map;
            this.faction = faction;
            base.free = free;
            targetingParameters.validator = delegate (TargetInfo target)
            {
                if (def.royalAid.targetingRange > 0f && target.Cell.DistanceTo(caller.Position) > def.royalAid.targetingRange)
                {
                    return false;
                }

                if (!target.Cell.Walkable(map))
                {
                    return false;
                }

                return (!target.Cell.Fogged(map)) ? true : false;
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
            if (!free)
            {
                caller.royalty.TryRemoveFavor(faction, def.royalAid.favorCost);
            }
            var ext = def.GetModExtension<AirSupportExtension>();
            if (ext != null)
            {
                int delay = Find.TickManager.TicksGame + ext.delayRange.RandomInRange;
                var c = GenRadial.NumCellsInRadius(ext.spreadRadius);
                var ori = CellFinder.RandomEdgeCell(caller.MapHeld).ToVector3Shifted();

                Thing triggerer = caller;

                for (int i = 0; i < ext.burstCount; i++)
                {
                    GameComponent_CAS.AddData(new AirSupportData_LaunchProjectile()
                    {
                        projectileDef = ext.projectileDef,
                        map = caller.MapHeld,
                        target = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + cell.Cell,
                        triggerTick = delay,
                        triggerer = triggerer,
                        triggerFaction = triggerer.Faction,
                        origin = ori
                    });
                    delay += ext.burstInterval;
                }
            }
        }
    }
}