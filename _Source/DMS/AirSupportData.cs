using RimWorld;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using Verse;
using Verse.Noise;
using Verse.Sound;
using static HarmonyLib.Code;

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

    public class AirSupportData_SpawnThing : AirSupportData
    {
        public ThingDef thingDef;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref thingDef, "thingDef");
        }

        public override void Trigger()
        {
            var t = ThingMaker.MakeThing(thingDef);
            PreProcess(t);
            GenSpawn.Spawn(thingDef, targetCell, map);
        }

        protected virtual void PreProcess(Thing t)
        {

        }
    }

    public class AirSupportData_SpawnFlyBy : AirSupportData_SpawnThing
    {
        public Vector3 origin;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
        }

        protected override void PreProcess(Thing t)
        {
            var flyby = t as FlyByThing;
            flyby.vector = targetCell.ToVector3Shifted() - origin;
        }
    }

    public class AirSupportData_Sound : AirSupportData
    {
        public SoundDef soundDef;

        public override void Trigger()
        {
            soundDef.PlayOneShot(SoundInfo.InMap(new TargetInfo(targetCell, map)));
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref soundDef, "soundDef");
        }
    }
}
