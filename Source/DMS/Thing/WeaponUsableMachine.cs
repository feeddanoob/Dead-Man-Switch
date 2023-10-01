using Verse;
using RimWorld;
using Verse.AI;
using VFE.Mechanoids;
using VFEMech;
using System.Collections.Generic;

namespace DMS
{
    
    //給VFEM機械使用的
    public class WeaponUsableMachine : Machine
    {
        public MechWeaponExtension MechWeapon { get; private set; }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.MechWeapon = this.def.GetModExtension<MechWeaponExtension>();
        }

        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            foreach (var item in base.GetExtraFloatMenuOptionsFor(sq))
            {
                yield return item;
            }
            if (this.Map == null)
            {
                Log.Error("Error");
                yield break;
            }
            List<Thing> things = sq.GetThingList(this.Map);

            for (int i = 0; i < things.Count; i++)
            {
                if (things[i] is ThingWithComps tmp)
                {
                    if (tmp == null) continue;
                    //沒有開啟武器過濾的情況下任意武器都應該能裝備//如果該裝備是可以使用的
                    if (tmp.TryGetComp<CompEquippable>() != null)
                    {
                        if (IsMechUseable(tmp))
                        {
                            yield return this.TryMakeFloatMenuForWeapon(tmp);
                        }
                        else
                        {
                            yield return new FloatMenuOption("CannotEquip".Translate(tmp) + "DMS_WeaponNotSupported".Translate(), null);
                        }
                    }
                }
            }
            yield break;
        }
        private bool IsMechUseable(ThingWithComps tmp)
        {
            //開了Tag過濾的話先看是否通過Tag過濾，然後InTechLevel包含了對於EnableTechLevelFilter的判斷
            if (this.MechWeapon.EnableWeaponFilter)
            {
                foreach (var item in this.MechWeapon.UsableWeaponTags)
                {
                    if (tmp.def.weaponTags.NotNullAndContains(item) && InTechLevel(tmp))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (this.MechWeapon.EnableTechLevelFilter)
            {
                return this.MechWeapon.UsableTechLevels.NotNullAndContains(tmp.def.techLevel.ToString());
            }
            return true;
        }
        private bool InTechLevel(ThingWithComps tmp)//為可用的科技等級。
        {
            if (!this.MechWeapon.EnableTechLevelFilter) return true;
            else return this.MechWeapon.UsableTechLevels.NotNullAndContains(tmp.def.techLevel.ToString());
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.Drawer?.renderer?.graphics.SetAllGraphicsDirty();
            }
        }
        public void Equip(ThingWithComps equipment)
        {
            equipment.SetForbidden(false);
            this.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Equip, equipment), JobTag.Misc);
        }
    }
}
