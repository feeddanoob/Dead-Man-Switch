using UnityEngine;
using Verse;
using Verse.Sound;

namespace DMS
{
    public class AirSupportData_LaunchProjectile : AirSupportData
    {
        public ThingDef projectileDef;
        public Vector3 origin;
        public SoundDef soundDef;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref projectileDef, "projectileDef");
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Defs.Look(ref soundDef, "soundDef");
        }

        public override void Trigger()
        {
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, origin.ToIntVec3(), map);
            projectile.Launch(triggerer, origin, target, target, ProjectileHitFlags.IntendedTarget);
            soundDef?.PlayOneShot(SoundInfo.InMap(new TargetInfo(origin.ToIntVec3(), map)));
        }
    }

    public class AirSupportData_LaunchProjectileFromPlane : AirSupportData_LaunchProjectile, IAttachedToFlyBy
    {
        public FlyByThing plane;

        public Vector3 offset;

        FlyByThing IAttachedToFlyBy.plane
        {
            set => plane = value;
        }

        public override void Trigger()
        {
            if (plane != null && plane.Spawned) origin = plane.DrawPos + offset.RotatedBy(plane.angle);
            base.Trigger();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref plane, "plane");
            Scribe_Values.Look(ref offset, "offset");
        }
    }
}
