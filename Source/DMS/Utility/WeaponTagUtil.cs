using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace DMS
{
    [StaticConstructorOnStartup]
    public static class WeaponTagUtil
    {
        static Dictionary<string, List<ThingDef>> dict = new Dictionary<string, List<ThingDef>>();

        static WeaponTagUtil()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.IsWeapon))
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
        }

        public static IEnumerable<ThingDef> getWeapons(List<string> tags)
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

                if (!users.NullOrEmpty())
                {
                    List<ThingDef> list = WeaponTagUtil.getWeapons(users).ToList();
                    list.SortBy(v => v.BaseMarketValue);
                    foreach (var def in list)
                    {
                        if (!ext.EnableTechLevelFilter || ext.UsableTechLevels.Contains(def.techLevel.ToString()))
                        {
                            yield return new Dialog_InfoCard.Hyperlink(def);
                        }
                    }
                }
            }
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
