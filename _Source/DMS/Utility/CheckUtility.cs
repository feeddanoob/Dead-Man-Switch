using Verse;
using DMS;
using System.Linq;
using System.Collections.Generic;
using RimWorld;
using System;

public static partial class CheckUtility
{
    public static bool IsMech(Pawn pawn)
    {
        bool flag1 = pawn.GetComp<CompOverseerSubject>() != null;
        bool flag2 = pawn.Faction == Faction.OfPlayer;
        return flag1 && flag2;
    }
    public static bool MechanitorCheck(Map map, out Pawn mechanitor)
    {
        mechanitor = null;
        if (map == null) return false;
        List<Pawn> colonists = map.mapPawns.FreeColonists;
        for (int i = 0; i < colonists.Count; i++)
        {
            if (MechanitorUtility.IsMechanitor(colonists[i]))
            {
                mechanitor = colonists[i];
                return true;
            }
        }
        return false;
    }
    public static bool IsMechUseable(Thing mech, ThingWithComps weapon)
    {
        if (IsMechUseable(mech.def.GetModExtension<MechWeaponExtension>(), weapon))//def定義上的可用
        {
            return true;
        }
        else if (UseableInRuntime(mech, weapon))//遊戲中透過改造所定義的可用
        {
            return true;
        }
        return false;
    }
    public static bool IsMechUseable(MechWeaponExtension extension, ThingWithComps thing)
    {
        if (extension == null)
        {
            return false;
        }
        if (thing == null)
        {
            return false;
        }
        if (BypassedUseable(extension, thing.def.defName)) return true;
        //開了Tag過濾的話先看是否通過Tag過濾，然後InTechLevel包含了對於EnableTechLevelFilter的判斷
        if (extension.EnableWeaponFilter)
        {
            foreach (var item in extension.UsableWeaponTags)
            {
                if (thing.def.weaponTags.NotNullAndContains(item) && InTechLevel(extension, thing))
                {
                    return true;
                }
            }
            return false;
        }
        else if (extension.EnableTechLevelFilter)
        {
            return InTechLevel(extension, thing);
        }
        else if (extension.EnableClassFilter)
        {
            foreach (var item in extension.UsableWeaponClasses)
            {
                return thing.def.weaponClasses?.Where(p => p == item).FirstOrDefault() != null;
            }
            return false;
        }
        return true;
    }
    public static bool UseableInRuntime(Thing mech, ThingWithComps weapon)//透過改造或別的因素所以可以用的狀況
    {
        HeavyEquippableExtension extension = weapon.def.GetModExtension<HeavyEquippableExtension>();
        if (extension == null) return true;

        HeavyEquippableDef eDef = extension.EquippableDef;
        Pawn pawn = mech as Pawn;
        return HasAnyHediffOf(pawn, eDef.EquippableWithHediff)|| CheckUtility.HasAnyApparelOf(pawn, eDef.EquippableWithApparel);
    }
    public static bool HasAnyHediffOf(Pawn pawn, List<HediffDef> hediffDefs)
    {
        if (pawn is null)
        {
            throw new ArgumentNullException(nameof(pawn));
        }

        foreach (var item in hediffDefs)
        {
            if (pawn.health.hediffSet.HasHediff(item)) return true;
        }
        return false;
    }
    public static bool HasAnyApparelOf(Pawn pawn, List<ThingDef> apparels)
    {
        if (pawn is null)
        {
            throw new ArgumentNullException(nameof(pawn));
        }

        foreach (ThingDef apparel in apparels)//裝備上可用
        {
            if (CheckUtility.WearsApparel(pawn, apparel)) return true;
        }
        return false;
    }
    public static bool Wearable(MechWeaponExtension extension, ThingWithComps equipment)//為可用的科技等級。
    {
        foreach (ApparelLayerDef item in extension.acceptedLayers)
        {
            if (equipment.def.apparel.layers.Contains(item))
            {
                return true;
            }
        }
        return false;
    }
    public static bool InTechLevel(MechWeaponExtension extension, ThingWithComps thing)//為可用的科技等級。
    {
        if (!extension.EnableTechLevelFilter) return true;
        else return extension.UsableTechLevels.NotNullAndContains(thing.def.techLevel);
    }

    public static bool IsMannable(TurretMannableExtension extension, Building_Turret turret)
    {
        if (extension == null) return false;
        if (turret is Building_Turret && turret.GetComp<CompMannable>() == null) return false;
        if (extension.mannableByDefault) return true;
        return extension.BypassMannable.NotNullAndContains(turret.def.defName);
    }
    public static bool BypassedUseable(MechWeaponExtension extension, string defName)//白名單直接可用
    {
        if (extension.BypassUsableWeapons.NullOrEmpty()) return false;
        return extension.BypassUsableWeapons?.FirstOrDefault(p => p == defName) != null;
    }
    public static bool WearsApparel(Pawn pawn, ThingDef thingDef)
    {
        if (pawn.apparel?.WornApparel != null)
        {
            return (pawn.apparel.WornApparel.Where(e => e.def == thingDef).FirstOrDefault() != null);
        }
        return false;
    }
    public static bool CanEquipHeavy(Pawn pawn, ThingWithComps thing)
    {
        HeavyEquippableExtension extension = thing.def.GetModExtension<HeavyEquippableExtension>();
        if (extension == null) return true;//沒有的話讓他過吧

        HeavyEquippableDef eDef = extension.EquippableDef;

        if (pawn.BodySize >= eDef.EquippableBaseBodySize && eDef.EquippableBaseBodySize != -1) return true;//體型上可用，如果為-1則關閉此判斷
        if (pawn is IWeaponUsable)//pawn是機兵，並且在他的MechWeaponExtension裡可以用這把槍
        {
            var ext = pawn.def.GetModExtension<MechWeaponExtension>();
            if (ext != null)
            {
                if (CheckUtility.IsMechUseable(pawn, thing)) return true;
                //if (ext.EnableWeaponFilter && CheckUtility.IsMechUseable(pawn, thing)) return true;
                //else if(pawn.BodySize>= eDef.EquippableBaseBodySize && eDef.EquippableBaseBodySize != -1) return true;//由於某些原因不限制時會需要再檢查一次。。
            }
        }
        if (eDef.EquippableByRace.Contains(pawn.def)) return true;//種族上可用
        if (ModsConfig.BiotechActive)
        {
            foreach (GeneDef gene in eDef.EquippableWithGene)//基因上可用
            {
                if (pawn.genes.GenesListForReading.Contains(GeneMaker.MakeGene(gene, pawn))) return true;
            }
        }
        if (CheckUtility.HasAnyHediffOf(pawn, eDef.EquippableWithHediff)) return true;//狀態上可用

        if (CheckUtility.HasAnyApparelOf(pawn, eDef.EquippableWithApparel)) return true;//裝備上可用

        return false;
    }
}
