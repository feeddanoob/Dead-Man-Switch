﻿using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using System.Linq;
using static RimWorld.MechClusterSketch;

namespace DMS
{
    public class WeaponUsableMech : Pawn, IWeaponUsable
    {
        public MechWeaponExtension MechWeapon { get; private set; }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            MechWeapon = def.GetModExtension<MechWeaponExtension>();
        }
        public override void Kill(DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (dinfo == null)//解體殺
            {
                List<Hediff> hediffs = health.hediffSet.hediffs.Where(h => h.def.spawnThingOnRemoved != null).ToList();
                foreach (Hediff item in hediffs)
                {
                    health.RemoveHediff(item);
                    Thing thing = ThingMaker.MakeThing(item.def.spawnThingOnRemoved);
                    thing.stackCount = 1;
                    GenPlace.TryPlaceThing(thing, this.Position, this.Map, ThingPlaceMode.Near);
                }
            }
            base.Kill(dinfo, exactCulprit);
        }
        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            if (IsColonyMechPlayerControlled)
            {
                foreach (var item in base.GetExtraFloatMenuOptionsFor(sq))
                {
                    yield return item;
                }
                foreach (var item in FloatMenuUtility.GetExtraFloatMenuOptionsFor(this, sq, MechWeapon))
                {
                    yield return item;
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Drawer?.renderer?.SetAllGraphicsDirty();
            }
        }
        public void Equip(ThingWithComps equipment)
        {
            equipment.SetForbidden(false);
            jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Equip, equipment), JobTag.Misc);
        }
        public void Wear(ThingWithComps apparel)
        {
            apparel.SetForbidden(false);
            this.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Wear, apparel), JobTag.Misc);
            this.equipment.AddEquipment(apparel);
        }
    }
}
