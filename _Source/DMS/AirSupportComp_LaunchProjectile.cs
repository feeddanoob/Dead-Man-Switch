using Mono.Unix.Native;
using RimWorld;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace DMS
{
    public class AirSupportComp_LaunchProjectile : AirSupportComp
    {
        public ThingDef ProjectileDef, flyByThingDef;

        public float spreadRadius = 5;

        public int burstCount = 1, burstInterval = 5, flyByDelay = 60;

        public IntRange delayRange = new IntRange(120, 150);

        public SoundDef soundDef;

        public override void Trigger(Thing triggerer, Map map, LocalTargetInfo target)
        {
            int delay = Find.TickManager.TicksGame + delayRange.RandomInRange;
            var c = GenRadial.NumCellsInRadius(spreadRadius);

            var ori = Position(Rand.Range(0, 360), map);

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
                    targetCell = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + target.Cell,
                    triggerTick = delay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                    origin = ori
                });

                if (soundDef != null) GameComponent_CAS.AddData(new AirSupportData_Sound()
                {
                    soundDef = soundDef,
                    map = map,
                    targetCell = ori.ToIntVec3(),
                    triggerTick = delay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                });

                delay += burstInterval;
            }

            if (flyByThingDef != null)
            {
                GameComponent_CAS.AddData(new AirSupportData_SpawnFlyBy()
                {
                    thingDef = flyByThingDef,
                    map = map,
                    targetCell = GenRadial.RadialPattern[Rand.RangeInclusive(0, c)] + target.Cell,
                    triggerTick = delay + flyByDelay,
                    triggerer = triggerer,
                    triggerFaction = triggerer.Faction,
                    origin = ori
                });
            }
        }

        protected Vector3 Position(float angle, Map map)
        {
            float theta = Mathf.Deg2Rad * angle; // 角度轉換為弧度

            // 計算方向向量
            float dx = Mathf.Sin(theta);
            float dy = -Mathf.Cos(theta);

            float x = map.Center.x;
            float y = map.Center.y;

            while (x >= 0 && x < map.Size.x && y >= 0 && y < map.Size.y)
            {
                x += dx;
                y += dy;
            }

            int edgeX = Mathf.RoundToInt(x - dx);
            int edgeY = Mathf.RoundToInt(y - dy);

            return new Vector3(edgeX, 0, edgeY);
        }
    }
}
