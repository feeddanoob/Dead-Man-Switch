using Verse;

namespace DMS
{
    public class Hediff_LevelLabel : Hediff_Level
    {
        public override string Label
        {
            get
            {
                if (!def.levelIsQuantity)
                {
                    return def.label + " (" + def.stages[level].label + ")";
                }
                return def.label + " x" + level;
            }
        }
    }
}
