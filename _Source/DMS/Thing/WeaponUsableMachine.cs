using Verse;
using RimWorld;
using Verse.AI;
using VFEMech;
using System.Collections.Generic;

namespace DMS
{
    //給VFEM機械使用的
    public class WeaponUsableMachine : Machine , IWeaponUsable
    {
        public MechWeaponExtension MechWeapon { get; private set; }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            MechWeapon = def.GetModExtension<MechWeaponExtension>();
        }
        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            foreach (var item in base.GetExtraFloatMenuOptionsFor(sq))
            {
                yield return item;
            }
            if (Map == null)
            {
                Log.Error("Error");
                yield break;
            }
            List<Thing> things = sq.GetThingList(Map);

            for (int i = 0; i < things.Count; i++)
            {
                if (things[i] is ThingWithComps tmp)
                {
                    if (tmp == null) continue;
                    //沒有開啟武器過濾的情況下任意武器都應該能裝備//如果該裝備是可以使用的
                    if (tmp.TryGetComp<CompEquippable>() != null)
                    {
                        if (CheckUtility.IsMechUseable(MechWeapon, tmp))
                        {
                            yield return this.TryMakeFloatMenuForWeapon(tmp);
                        }
                        else
                        {
                            yield return new FloatMenuOption("CannotEquip".Translate(tmp) + "DMS_WeaponNotSupported".Translate(), null);
                        }
                    }
                    if (tmp.def?.apparel != null && MechWeapon.acceptedLayers?.Count > 0)
                    {
                        if (CheckUtility.Wearable(MechWeapon, tmp))
                        {
                            yield return this.TryMakeFloatMenuForApparel(tmp);
                        }
                        else
                        {
                            yield return new FloatMenuOption("CannotEquip".Translate(tmp) + "DMS_FrameNotSupported".Translate(), null);
                        }
                    }
                    //操作砲塔相關
                    else if (CheckUtility.IsMannable(def.GetModExtension<TurretMannableExtension>(), tmp as Building_Turret))
                    {
                        var turret = tmp as Building_Turret;
                        yield return new FloatMenuOption("OrderManThing".Translate(turret.LabelShort, turret), delegate
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.ManTurret, turret);
                            jobs.TryTakeOrderedJob(job, JobTag.DraftedOrder);
                        });
                    }
                }
            }
            yield break;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Drawer?.renderer?.graphics.SetAllGraphicsDirty();
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
        }
    }
}
