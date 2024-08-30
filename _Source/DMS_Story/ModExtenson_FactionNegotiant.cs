using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DMS_Story
{
    public class ModExtenson_FactionNegotiant : DefModExtension
    {
        public int tickToRefreshGoods = 60000;
        public List<ThingSetMakerDef> goods = new List<ThingSetMakerDef>();
        public TraderKindDef traderKind;
        public RulePackDef nameRule;
    }
}
