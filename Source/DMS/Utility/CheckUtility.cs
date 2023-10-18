using Verse;
using DMS;

public static class CheckUtility
{
    public static bool IsMechUseable(MechWeaponExtension extension, ThingWithComps tmp)
    {
        //開了Tag過濾的話先看是否通過Tag過濾，然後InTechLevel包含了對於EnableTechLevelFilter的判斷
        if (extension.EnableWeaponFilter)
        {
            foreach (var item in extension.UsableWeaponTags)
            {
                if (tmp.def.weaponTags.NotNullAndContains(item) && InTechLevel(extension,tmp))
                {
                    return true;
                }
            }
            return false;
        }
        else if (extension.EnableTechLevelFilter)
        {
            return extension.UsableTechLevels.NotNullAndContains(tmp.def.techLevel.ToString());
        }
        return true;
    }
    public static bool InTechLevel(MechWeaponExtension extension, ThingWithComps tmp)//為可用的科技等級。
    {
        if (!extension.EnableTechLevelFilter) return true;
        else return extension.UsableTechLevels.NotNullAndContains(tmp.def.techLevel.ToString());
    }
}
