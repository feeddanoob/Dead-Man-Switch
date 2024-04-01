using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class HeavyEquippableExtension : DefModExtension
    {
        public float EquippableBaseBodySize = 1;
        public List<ThingDef> EquippableByRaces;//指定種族可以無視體型需求直接裝備
        public List<ThingDef> EquippableWithApparel;//穿戴指定衣物時可以無視體型需求直接裝備
    }
}
