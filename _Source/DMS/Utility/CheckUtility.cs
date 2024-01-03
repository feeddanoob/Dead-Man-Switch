using Verse;
using DMS;
using System.Linq;

public static partial class CheckUtility
{
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
                return tmp.def.weaponClasses?.Where(p => p.defName == item).FirstOrDefault() != null;
            }
            return false;
        }
        return true;
    }
    public static bool InTechLevel(MechWeaponExtension extension, ThingWithComps tmp)//為可用的科技等級。
    {
        if (!extension.EnableTechLevelFilter) return true;
        else return extension.UsableTechLevels.NotNullAndContains(tmp.def.techLevel.ToString());
    }

    public static bool BypassedUseable(MechWeaponExtension extension, string defName)//白名單直接可用
    {
        return (extension.BypassUsableWeapons.Where(p => p == defName).FirstOrDefault() != null);
    }
}
