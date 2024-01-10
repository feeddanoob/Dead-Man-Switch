using Verse;
using DMS;
using System.Linq;
using System.Collections.Generic;
using RimWorld;

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
    public static bool IsMechUseable(MechWeaponExtension extension, ThingWithComps tmp)
    {
        if (BypassedUseable(extension, tmp.def.defName)) return true;
        //開了Tag過濾的話先看是否通過Tag過濾，然後InTechLevel包含了對於EnableTechLevelFilter的判斷
        if (extension.EnableWeaponFilter)
        {
            foreach (var item in extension.UsableWeaponTags)
            {
                if (tmp.def.weaponTags.NotNullAndContains(item) && InTechLevel(extension, tmp))
                {
                    return true;
                }
            }
            return false;
        }

        else if (extension.EnableTechLevelFilter)
        {
            return InTechLevel(extension, tmp);
        }

        else if (extension.EnableClassFilter)
        {
            foreach (var item in extension.UsableWeaponClasses)
            {
                return tmp.def.weaponClasses?.Where(p => p == item).FirstOrDefault() != null;
            }
            return false;
        }
        return true;
    }
    public static bool Wearable(MechWeaponExtension extension, ThingWithComps tmp)//為可用的科技等級。
    {
        foreach (ApparelLayerDef item in extension.acceptedLayers)
        {
            if (tmp.def.apparel.layers.Contains(item))
            {
                return true;
            }
        }
        return false;
    }
    public static bool InTechLevel(MechWeaponExtension extension, ThingWithComps tmp)//為可用的科技等級。
    {
        if (!extension.EnableTechLevelFilter) return true;
        else return extension.UsableTechLevels.NotNullAndContains(tmp.def.techLevel);
    }

    public static bool IsMannable(TurretMannableExtension extension, Building_Turret tmp)
    {
        if (extension == null) return false;
        if (tmp is Building_Turret && tmp.GetComp<CompMannable>() == null) return false;
        if (extension.mannableByDefault) return true;
        return extension.BypassMannable.NotNullAndContains(tmp.def.defName);
    }
    public static bool BypassedUseable(MechWeaponExtension extension, string defName)//白名單直接可用
    {
        return extension.BypassUsableWeapons?.FirstOrDefault(p => p == defName) != null;
    }
}
