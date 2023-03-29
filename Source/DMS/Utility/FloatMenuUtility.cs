using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace DMS
{
    public static class FloatMenuUtility
    {
        public static FloatMenuOption TryMakeFloatMenuForWeapon(this HumanlikeMech pawn, ThingWithComps equipment)
        {
            string labelShort = equipment.LabelShort;
            if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, false, TraverseMode.ByPawn))
            {
                return new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }

            return new FloatMenuOption("Equip".Translate(labelShort), () =>
            {
                pawn.Equip(equipment);
            });
        }

        public static FloatMenuOption TryMakeFloatMenuForApparel(this HumanlikeMech pawn, ThingWithComps equipment)
        {
            string labelShort = equipment.LabelShort;
            if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, false, TraverseMode.ByPawn))
            {
                return new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }

            string key = equipment.def.apparel.LastLayer.IsUtilityLayer ? "ForceWear" : "ForceEquipApparel";

            return new FloatMenuOption(key.Translate(labelShort, equipment), () =>
            {
                pawn.Wear(equipment);
            });
        }
    }
}
