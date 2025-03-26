using RimWorld;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using Verse;
using Verse.Noise;

namespace DMS
{
    public abstract class AirSupportData : IExposable
    {
        public Map map;

        public IntVec3 targetCell;

        public int triggerTick;

        public Thing triggerer;

        public Faction triggerFaction;

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_References.Look(ref triggerer, "triggerer");
            Scribe_References.Look(ref triggerFaction, "triggerFaction");
            Scribe_Values.Look(ref targetCell, "cell");
            Scribe_Values.Look(ref triggerTick, "triggerTick");
        }

        public abstract void Trigger();
    }

    public class AirSupportData_LaunchProjectile : AirSupportData
    {
        public ThingDef projectileDef;
        public Vector3 origin;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref projectileDef, "projectileDef");
            Scribe_Values.Look(ref origin, "origin");
        }
        public void Trigger(float angle)
        {
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, Position(angle, map), map);
            projectile.Launch(triggerer, Position(angle, map).ToVector3(), targetCell, targetCell, ProjectileHitFlags.IntendedTarget);
        }
        private IntVec3 Position(float angle, Map map)
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

            return new IntVec3(edgeX, 0, edgeY);
        }

        public override void Trigger()
        {
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, origin.ToIntVec3(), map);
            projectile.Launch(triggerer, origin, targetCell, targetCell, ProjectileHitFlags.IntendedTarget);
        }
    }
}
