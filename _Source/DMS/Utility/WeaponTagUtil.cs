using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace DMS
{
    [StaticConstructorOnStartup]
    public static class WeaponTagUtil
    {
        static readonly Dictionary<string, List<ThingDef>> AllTags = new Dictionary<string, List<ThingDef>>();//AllTags;
        static readonly ThingDef[] Turrets = new ThingDef[0];
        static readonly ThingDef[] WeaponUseableMechs = new ThingDef[0];//所有能切裝備的機兵
        static readonly ThingDef[] AllWeaponDefs = new ThingDef[0];//所有武器
        public static ThingDef[] GetTurrets => Turrets;
        static WeaponTagUtil()
        {
            AllWeaponDefs = DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.IsWeapon).ToArray();
            foreach (ThingDef def in AllWeaponDefs)//整理所有Tag與對應的所有武器
            {
                if (def.weaponTags.NullOrEmpty())
                {
                    continue;
                }
                foreach (string tag in def.weaponTags.Distinct())
                {
                    if (AllTags.ContainsKey(tag))
                    {
                        AllTags[tag].Add(def);
                    }
                    else if (!string.IsNullOrEmpty(tag))
                    {
                        AllTags.Add(tag, new List<ThingDef>() { def });
                    }
                }
            }

            List<ThingDef> _ts = new List<ThingDef>();//砲塔
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef a) => a.building?.turretGunDef != null))
            {
                if (def.GetCompProperties<CompProperties_Mannable>() != null)
                {
                    _ts.Add(def);
                }
            }
            _ts.RemoveDuplicates();
            _ts.SortBy(v => v.BaseMass);
            Turrets = _ts.ToArray();
            _ts = new List<ThingDef>();

            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef a) => a.GetModExtension<MechWeaponExtension>() != null))
            {
                _ts.Add(def);
            }
            _ts.RemoveDuplicates();
            _ts.SortBy(v => v.BaseMass);
            WeaponUseableMechs = _ts.ToArray();
        }

        public static IEnumerable<ThingDef> GetWeapons(List<string> tags)
        {
            List<ThingDef> thingDefs = new List<ThingDef>();
            foreach (string s in tags)
            {
                if (AllTags.ContainsKey(s))
                {
                    thingDefs = thingDefs.ConcatIfNotNull(AllTags[s]).ToList();
                }
            }
            thingDefs.RemoveDuplicates();
            return thingDefs;
        }
        public static bool WeaponExists(string defname, out ThingDef thing)
        {
            thing = DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.IsWeapon && def.defName == defname).FirstOrDefault();
            return thing != null;
        }
        public static bool WeaponExistsInTurretDict(string defname, out ThingDef thing)
        {
            thing = Turrets.Where(t => t.defName == defname).FirstOrDefault();
            return thing != null;
        }
        public static ThingDef[] UseableByListsOfMechs(ThingWithComps weapon)//重型裝備判斷是否能夠被機械體使用
        {
            List<ThingDef> list = new List<ThingDef>();
            foreach (ThingDef mech in WeaponUseableMechs)
            {
                MechWeaponExtension ext = mech.GetModExtension<MechWeaponExtension>();
                if (ext != null && CheckUtility.IsMechUseable(ext, weapon))
                {
                    list.Add(mech);
                }
                else if (!ext.EnableWeaponFilter)
                {
                    var _ext = weapon.def.GetModExtension<HeavyEquippableExtension>();
                    if (_ext != null && _ext.EquippableDef != null)
                    {
                        if (_ext.CanEquippedBy(ThingMaker.MakeThing(mech) as Pawn))
                        {
                            list.Add(mech);
                        }
                    }
                }
            }
            list.RemoveDuplicates();
            return list.ToArray();
        }
    }
}
