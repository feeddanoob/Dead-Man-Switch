using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace DMS
{
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
