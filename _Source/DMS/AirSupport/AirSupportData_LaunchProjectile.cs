using UnityEngine;
using Verse;

namespace DMS
{
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
            projectile.Launch(triggerer, origin, target, target, ProjectileHitFlags.IntendedTarget);
        }
    }
}
