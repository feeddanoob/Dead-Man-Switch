using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace DMS
{
    public abstract class AirSupportData : IExposable
    {
        public Thing triggerer;
        public Faction triggerFaction;

        public Map map;
        public Vector3 origin;//生成位置
        public IntVec3 targetCell;

        public SupportDef def;

        protected Effecter preEffecter;
        protected Effecter triggerEffecter;
        protected Effecter postEffecter;

        public int triggerTick; //觸發時間
        public float TriggerSecond => (triggerTick - Find.TickManager.TicksGame).TicksToSeconds();
        public virtual void TickEffecter()
        {
            if (triggerTick - def.preTriggerTick == Find.TickManager.TicksGame)
            {
                if (def.preEffecterDef != null)
                {
                    preEffecter ??= def.preEffecterDef.Spawn();
                    preEffecter.Trigger(new TargetInfo(origin.ToIntVec3(), map), new TargetInfo(targetCell, map));
                }
            }
            else if (triggerTick == Find.TickManager.TicksGame)
            {
                if (def.triggerEffecter != null)
                {
                    triggerEffecter ??= def.triggerEffecter.Spawn();
                    triggerEffecter.Trigger(new TargetInfo(origin.ToIntVec3(), map), new TargetInfo(targetCell, map));
                }
                    
            }
            else if (triggerTick + def.postTriggerTick == Find.TickManager.TicksGame)
            {
                if (def.postEffecterDef != null)
                {
                    postEffecter ??= def.postEffecterDef.Spawn();
                    postEffecter.Trigger(new TargetInfo(origin.ToIntVec3(), map), new TargetInfo(targetCell, map));
                }
            }
        }

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_References.Look(ref triggerer, "triggerer");
            Scribe_References.Look(ref triggerFaction, "triggerFaction");
            Scribe_Values.Look(ref targetCell, "cell");
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref triggerTick, "triggerTick");
            Scribe_Values.Look(ref origin, "origin");
        }

        public abstract void Trigger();
    }

    public class AirSupportData_LaunchProjectile : AirSupportData
    {
        public ThingDef projectileDef;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref projectileDef, "projectileDef");
        }
        public override void Trigger()
        {
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, origin.ToIntVec3(), map);
            projectile.Launch(triggerer, origin, targetCell, targetCell, ProjectileHitFlags.IntendedTarget);
        }
    }
}