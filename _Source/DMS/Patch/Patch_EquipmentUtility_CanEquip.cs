using HarmonyLib;
using RimWorld;
using System;
using Verse;
using VFECore;

namespace DMS
{
    [HarmonyPatch(
        typeof(EquipmentUtility),
        nameof(EquipmentUtility.CanEquip),
        new Type[] { typeof(Thing), typeof(Pawn), typeof(string), typeof(bool) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    internal static class Patch_EquipmentUtility_CanEquip
    {
        static void Postfix(ref bool __result, object __0, object __1, object __2, object __3)
        {
            if (!(__0 is Thing thing) || !(__1 is Pawn pawn)) return;
            if (thing.TryGetComp<CompEquippable>() != null)
            {
                //var extension = thing.def.GetModExtension<HeavyEquippableExtension>();
                //if (extension != null)
                //{
                //    if (CheckUtility.CanEquipHeavy(pawn, thing as ThingWithComps))
                //    {
                //        __result = true;
                //    }
                //    else
                //    {
                //        __2 = "CannotEquip_TooHeavy".Translate();
                //        __result = false;
                //    }
                //}
                if (CheckUtility.IsMechUseable(pawn, thing as ThingWithComps))
                {
                    __result = true;
                }
                else
                {
                    __2 = "CannotEquip_TooHeavy".Translate();
                    __result = false;
                }
            }
        }
    }
}
