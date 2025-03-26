using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class AirSupportDef : Def
    {
        public List<AirSupportComp> comps = new List<AirSupportComp>();

        public void Trigger(Thing trigger, Map map, LocalTargetInfo target)
        {
            foreach (AirSupportComp comp in comps)
            {
                comp.Trigger(trigger, map, target);
            }
        }
    }
}
