using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class CompMechOnlyWeapon : ThingComp
    {
        public CompProperties_MechOnlyWeapon Props
        {
            get
            {
                return (CompProperties_MechOnlyWeapon)props;
            }
        }
        public bool IsAllowedRaces(string race)
        {
            return Props.allowedRaces.NotNullAndContains(race);
        }
    }
    public class CompProperties_MechOnlyWeapon : CompProperties
    {
        public CompProperties_MechOnlyWeapon()
        {
            this.compClass = typeof(CompMechOnlyWeapon);
        }
        public List<string> allowedRaces = new List<string>();
    }
}
