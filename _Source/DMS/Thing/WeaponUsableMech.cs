using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using System;

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
