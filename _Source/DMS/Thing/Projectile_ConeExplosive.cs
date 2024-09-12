using RimWorld;
using UnityEngine;
using Verse;
using static RimWorld.FleshTypeDef;

namespace DMS
{
    public class Projectile_ConeExplosive : Projectile_Explosive
    {
        protected float saftyRange = 5;
        private float cacheSqrRange = 1;
        protected float TravelDistance => (DrawPos - origin).sqrMagnitude;
        protected float coneSway = 10f;
        protected Vector3 Angle => (destination - origin).normalized;
        protected float Sway => Angle.RotatedBy(coneSway).ToAngleFlat();
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (this.def.HasModExtension<ExplosiveExtension>())
            {
                var ext = this.def.GetModExtension<ExplosiveExtension>();
                cacheSqrRange = Mathf.Pow(ext.saftyRange,2);
            }
        }
        protected void DoExplosion()
        {
            if (this.def.HasModExtension<ExplosiveExtension>())
            {
                var ext = this.def.GetModExtension<ExplosiveExtension>();
                if (TravelDistance < cacheSqrRange) return;
                GenExplosion.DoExplosion(Position - (Angle * ext.preExplosionOffset).ToIntVec3(), this.Map, ext.range,
                ext.damage, this.launcher,
                    15, 0.5f, ext.sound, this.launcher.def,
                    direction: Angle.ToAngleFlat(), affectedAngle: new FloatRange(-ext.swayAngle, ext.swayAngle),
                    doVisualEffects: ext.doVisualEffects, doSoundEffects: ext.sound != null
                    );
            }
            else //默認值
            {
                if (TravelDistance < cacheSqrRange) return;
                GenExplosion.DoExplosion(Position - (Angle * 2).ToIntVec3(), this.Map, 7,
                    DamageDefOf.Bullet, this.launcher,
                    30, 0.5f, weapon: launcher.def,
                    direction: Angle.ToAngleFlat(), affectedAngle: new FloatRange(-Sway, Sway),
                    doVisualEffects: true, doSoundEffects: false
                    );
            }
        }
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            DoExplosion();
            base.Destroy(mode);
        }
    }
    public class ExplosiveExtension : DefModExtension
    {
        public DamageDef damage = null;
        public float saftyRange = 0;
        public float preExplosionOffset = 0;
        public float range = 0;
        public float swayAngle = 0;
        public SoundDef sound = null;
        public bool doVisualEffects = false;
    }
}