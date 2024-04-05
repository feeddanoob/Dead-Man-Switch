using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace DMS
{
    [StaticConstructorOnStartup]
    public static class WeaponTagUtil
    {
        static readonly Dictionary<string, List<ThingDef>> dict = new Dictionary<string, List<ThingDef>>();
        static readonly List<ThingDef> turrets = new List<ThingDef>();
        static readonly List<ThingDef> weaponUseableMechs = new List<ThingDef>();

        public static List<ThingDef> Turrets => turrets;
        static WeaponTagUtil()
        {
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.IsWeapon))
            {
                if (def.weaponTags.NullOrEmpty())
                {
                    continue;
                }
                foreach (string tag in def.weaponTags)
                {
                    if (dict.ContainsKey(tag))
                    {
                        dict[tag].Add(def);
                    }
                    else if (tag != null)
                    {
                        dict.Add(tag, new List<ThingDef>() { def });
                    }
                }
            }
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef a) => a.building?.turretGunDef != null))
            {
                if (def.GetCompProperties<CompProperties_Mannable>() != null)
                {
                    turrets.Add(def);
                    turrets.SortBy(v => v.BaseMarketValue);
                }
            }
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef a) => a.GetModExtension<MechWeaponExtension>()!=null))
            {
                weaponUseableMechs.Add(def);
                weaponUseableMechs.SortBy(v => v.label);
            }
        }

        public static IEnumerable<ThingDef> GetWeapons(List<string> tags)
        {
            List<ThingDef> thingDefs = new List<ThingDef>();
            foreach (string s in tags)
            {
                if (dict.ContainsKey(s))
                {
                    thingDefs = thingDefs.ConcatIfNotNull(dict[s]).ToList();
                }
            }
            thingDefs.RemoveDuplicates();
            return thingDefs;
        }
        public static bool WeaponExists(string defname ,out ThingDef thing)
        {
            thing = null;
            foreach (var item in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.IsWeapon))
            {
                if (item.defName == defname)
                {
                    thing = item;
                    return true;
                } 
            }
            return false;
        }
        public static bool WeaponExistsInTurretDict(string defname, out ThingDef thing)
        {
            thing = null;
            foreach (ThingDef item in turrets)
            {
                if (item.defName == defname)
                {
                    thing = item;
                    return true;
                }
            }
            return false;
        }
        public static ThingDef[] UseableByListsOfMechs(ThingWithComps weapon)
        {
            List<ThingDef> list = new List<ThingDef>();
            ;
            foreach (ThingDef mech in weaponUseableMechs)
            {
                MechWeaponExtension ext = mech.GetModExtension<MechWeaponExtension>();
                if (ext != null && CheckUtility.IsMechUseable(ext, weapon))
                {
                    list.Add(mech);
                }
            }
            return list.ToArray();
        }
    }
}
