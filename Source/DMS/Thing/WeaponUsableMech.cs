using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace DMS
{
    public class MechWeaponExtension : DefModExtension
    {
        public bool EnableWeaponFilter = true;
        public List<string> UsableWeaponTags = new List<string>();
        public bool EnableTechLevelFilter = false;
        public List<string> UsableTechLevels = new List<string>();
    }
    public class WeaponUsableMech : Pawn
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
                    if (tmp.TryGetComp<CompEquippable>() != null && IsMechUseable(tmp))//
                    {
                        yield return this.TryMakeFloatMenuForWeapon(tmp);
                    }
                }
            }
            yield break;
        }
        private bool IsMechUseable(ThingWithComps tmp)
        {
           //開了Tag過濾的話先看是否通過Tag過濾，然後InTechLevel包含了對於EnableTechLevelFilter的判斷
            if (MechWeapon.EnableWeaponFilter)
            {
                foreach (var item in MechWeapon.UsableWeaponTags)
                {
                    Log.Warning(tmp.def.defName + "," + InTechLevel(tmp) + "," + (tmp.def.weaponTags.NotNullAndContains(item) && InTechLevel(tmp)));
                    if (tmp.def.weaponTags.NotNullAndContains(item)&& InTechLevel(tmp))
                    {

                        return true;
                    }
                }
            }
            return InTechLevel(tmp);
        }
        private bool InTechLevel(ThingWithComps tmp)//沒開或者為可用的科技等級。
        {
            if (MechWeapon.EnableTechLevelFilter)
                return MechWeapon.UsableTechLevels.NotNullAndContains(tmp.def.techLevel.ToString());
            return true;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if(Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.Drawer?.renderer?.graphics.SetAllGraphicsDirty();
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void Equip(ThingWithComps equipment)
        {
            equipment.SetForbidden(false);
            this.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Equip, equipment), JobTag.Misc);
        }
    }
}
