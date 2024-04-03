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
    public class StatWorker_HeavyGear : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            return base.ShouldShowFor(req) && req.Def.HasModExtension<HeavyEquippableExtension>();
        }
        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            HeavyEquippableExtension ext = statRequest.Def.GetModExtension<HeavyEquippableExtension>();
            if (ext != null)
            {
                foreach (HediffDef hediff in ext.EquippableDef.EquippableWithHediff)
                {
                    yield return new Dialog_InfoCard.Hyperlink(hediff);
                }
                foreach (ThingDef apparel in ext.EquippableDef.EquippableWithApparel)
                {
                    yield return new Dialog_InfoCard.Hyperlink(apparel);
                }
                if (ModsConfig.BiotechActive)
                {
                    foreach (GeneDef gene in ext.EquippableDef.EquippableWithGene)
                    {
                        yield return new Dialog_InfoCard.Hyperlink(gene);
                    }
                }
                foreach (ThingDef race in ext.EquippableDef.EquippableByRace)
                {
                    yield return new Dialog_InfoCard.Hyperlink(race);
                }
                foreach (ThingDef race in WeaponTagUtil.UseableByListsOfMechs(statRequest.Thing as ThingWithComps))
                {
                    yield return new Dialog_InfoCard.Hyperlink(race);
                }
            }
        }
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            return "";
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            if (req.Def.GetModExtension<HeavyEquippableExtension>().EquippableDef.EquippableBaseBodySize == -1)
            {
                return "DMS_MountedWeaponCanOnlyBeEquippedBySpecificApparelOrRaces".Translate();
            }
            return "DMS_WeaponCanBeEquippedBySpecificApparelOrRaces".Translate();
        }
        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            if (optionalReq.Def.GetModExtension<HeavyEquippableExtension>().EquippableDef.EquippableBaseBodySize == -1)
            {
                return "DMS_MountedWeapon".Translate();
            }
            return optionalReq.Def.GetModExtension<HeavyEquippableExtension>().EquippableDef.EquippableBaseBodySize.ToString("0.##");
        }
    }
    public class StatWorker_MechWeapon : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            return base.ShouldShowFor(req) && req.Def.HasModExtension<MechWeaponExtension>();
        }

        public override IEnumerable<Dialog_InfoCard.Hyperlink> GetInfoCardHyperlinks(StatRequest statRequest)
        {
            MechWeaponExtension ext = statRequest.Def.GetModExtension<MechWeaponExtension>();
            if (ext != null)
            {
                var users = ext.UsableWeaponTags;
                List<ThingDef> list = WeaponTagUtil.GetWeapons(users).ToList();
                list.SortBy(v => v.BaseMarketValue);

                if (!users.NullOrEmpty())
                {
                    foreach (var def in list)
                    {
                        if (!ext.EnableTechLevelFilter || ext.UsableTechLevels.Contains(def.techLevel))
                        {
                            yield return new Dialog_InfoCard.Hyperlink(def);
                        }

                    }
                }
                if (!ext.BypassUsableWeapons.NullOrEmpty())
                {
                    foreach (var item in ext.BypassUsableWeapons)
                    {
                        if (WeaponTagUtil.WeaponExists(item, out var def))
                        {
                            yield return new Dialog_InfoCard.Hyperlink(def);
                        }
                    }
                }
            }
            TurretMannableExtension ext2 = statRequest.Def.GetModExtension<TurretMannableExtension>();
            if (ext2 != null)
            {
                if (ext2.mannableByDefault)
                {
                    foreach (ThingDef item in WeaponTagUtil.Turrets)
                    {
                        yield return new Dialog_InfoCard.Hyperlink(item);
                    }
                }
                else if (!ext2.BypassMannable.NullOrEmpty())
                {
                    foreach (string item in ext2.BypassMannable)
                    {
                        if (WeaponTagUtil.WeaponExistsInTurretDict(item, out ThingDef def))
                        {
                            yield return new Dialog_InfoCard.Hyperlink(def);
                        }
                    }
                }
            }
        }

        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            return "";
        }
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            return "";
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            return "";
        }
    }
}
