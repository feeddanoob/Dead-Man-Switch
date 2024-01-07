using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class TurretMannableExtension : DefModExtension
    {
        public bool mannableByDefault = true;
        public bool filterByTechLevel = false;  //根據科技等級

        public List<TechLevel> techLevels = new List<TechLevel>();
        public List<string> BypassMannable = new List<string>();//特例可以直接用的
    }
}
