using RimWorld;
using UnityEngine;
using Verse;

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

        public override void Trigger()
        {
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, origin.ToIntVec3(), map);
            projectile.Launch(triggerer, origin, targetCell, targetCell, ProjectileHitFlags.IntendedTarget);
        }
    }
}
