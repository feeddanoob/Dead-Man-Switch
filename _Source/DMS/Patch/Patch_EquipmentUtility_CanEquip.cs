using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
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
                var extension = thing.def.GetModExtension<HeavyEquippableExtension>();
                if (extension != null)
                {
                    if (CanEquipHeavy(pawn, extension))
                    {
                        __result = true;
                    }
                    else
                    {
                        __2 = "CannotEquip_TooHeavy"; 
                        __result = false;
                    }

                }
            }
        }

        static bool CanEquipHeavy(Pawn pawn, HeavyEquippableExtension extension)
        {
            
            if (extension == null) return false;
            if (pawn.BodySize >= extension.EquippableDef.EquippableBaseBodySize && extension.EquippableDef.EquippableBaseBodySize !=-1) return true;//體型上可用，如果為-1則關閉此判斷
            if (pawn is IWeaponUsable)//機兵操作上可用
            {
                var ext = pawn.def.GetModExtension<MechWeaponExtension>();
                if (ext != null && CheckUtility.IsMechUseable(ext, pawn)) return true;
            }
            if (extension.EquippableDef.EquippableByRace.Contains(pawn.def)) return true;//種族上可用
            if (ModsConfig.BiotechActive)
            {
                foreach (GeneDef gene in extension.EquippableDef.EquippableWithGene)//基因上可用
                {
                    if (pawn.genes.GenesListForReading.Contains(GeneMaker.MakeGene(gene,pawn))) return true;
                }
            }       
            foreach (HediffDef hediff in extension.EquippableDef.EquippableWithHediff)//仿生上可用
            {
                if (pawn.health.hediffSet.GetFirstHediffOfDef(hediff) != null) return true;
            }
            foreach (ThingDef apparel in extension.EquippableDef.EquippableWithApparel)//裝備上可用
            {
                if (pawn.WearsApparel(apparel)) return true;
            }
            
            return false;
        }
    }
}
