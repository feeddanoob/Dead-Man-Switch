using Mono.Unix.Native;
using RimWorld;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace DMS
{

    public class AirSupportComp_Strafe : AirSupportComp
    {
        public ThingDef ProjectileDef, flyByThingDef;

        public float spreadRadius = 5;

        public int burstCount = 1, burstInterval = 5, flyByDelay = 60;

        public IntRange delayRange = new(120, 150);

        public SoundDef soundDef;

        public override void Trigger(AirSupportDef def, Thing triggerer, Map map, LocalTargetInfo target)
        {
            int delay = Find.TickManager.TicksGame + delayRange.RandomInRange;
            var c = GenRadial.NumCellsInRadius(spreadRadius);

            for (int i = 0; i < burstCount; i++)
            {
                var cell = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + target.Cell;
                if (!cell.InBounds(map))
                {
                    i--;
                    continue;
                }

                GameComponent_CAS.AddData(new AirSupportData_LaunchProjectile()
                {
                    projectileDef = ProjectileDef,
                    map = map,
                    target = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + target.Cell,
                    triggerTick = delay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                    origin = def.tempOriginCache
                });

                if (soundDef != null) GameComponent_CAS.AddData(new AirSupportData_Sound()
                {
                    soundDef = soundDef,
                    map = map,
                    target = def.tempOriginCache.ToIntVec3(),
                    triggerTick = delay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                });

                delay += burstInterval;
            }
            //I need to know when the burst ends, so this have to be integrated.
            if (flyByThingDef != null)
            {
                GameComponent_CAS.AddData(new AirSupportData_SpawnFlyBy()
                {
                    thingDef = flyByThingDef,
                    map = map,
                    target = target.Cell,
                    triggerTick = delay + flyByDelay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                    origin = def.tempOriginCache
                });
            }
        }
    }
}
