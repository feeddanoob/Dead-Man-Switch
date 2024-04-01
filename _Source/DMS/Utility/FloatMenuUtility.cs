using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;

namespace DMS
{
    public static class FloatMenuUtility
    {
        public static IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(Pawn pawn, IntVec3 sq, MechWeaponExtension MechWeapon)
        {
            IWeaponUsable weaponUsable = pawn as IWeaponUsable;
            if (pawn.Map == null)
            {
                Log.Error("Error");
                yield break;
            }
            List<Thing> things = sq.GetThingList(pawn.Map);

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
                            yield return TryMakeFloatMenuForWeapon(pawn, tmp);
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
                            yield return TryMakeFloatMenuForApparel(pawn, tmp);
                        }
                        else
                        {
                            yield return new FloatMenuOption("CannotEquip".Translate(tmp) + "DMS_FrameNotSupported".Translate(), null);
                        }
                    }
                    //操作砲塔相關
                    if (tmp.def.building?.turretGunDef != null)
                    {
                        if (CheckUtility.IsMannable(pawn.def.GetModExtension<TurretMannableExtension>(), tmp as Building_Turret))
                        {
                            var turret = tmp as Building_Turret;
                            yield return new FloatMenuOption("OrderManThing".Translate(turret.LabelShort, turret), delegate
                            {
                                Job job = JobMaker.MakeJob(JobDefOf.ManTurret, turret);
                                pawn.jobs.TryTakeOrderedJob(job, JobTag.DraftedOrder);
                            });
                        }
                    }
                }
            }
            yield break;
        }

        public static FloatMenuOption TryMakeFloatMenu(Pawn pawn, ThingWithComps equipment, string key = "Equip")
        {
            string labelShort = equipment.LabelShort;
            if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, false, TraverseMode.ByPawn))
            {
                return new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }

            if (pawn is IWeaponUsable weaponUsable)
            {
                return new FloatMenuOption(key.Translate(labelShort, equipment), () =>
                {
                    weaponUsable.Equip(equipment);
                });
            }
            return null;
        }
        public static FloatMenuOption TryMakeFloatMenuForWeapon(this Pawn pawn, ThingWithComps equipment)
        {
            return TryMakeFloatMenu(pawn, equipment);
        }
        public static FloatMenuOption TryMakeFloatMenuForApparel(this Pawn pawn, ThingWithComps equipment)
        {
            string key = equipment.def.apparel.LastLayer.IsUtilityLayer ? "ForceWear" : "ForceEquipApparel";
            return TryMakeFloatMenu(pawn, equipment, key);
        }
    }
}
