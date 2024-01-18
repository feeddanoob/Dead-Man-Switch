using Verse;

namespace DMS
{
    public class CompMechBillWroker : ThingComp
    {
        public CompProperties_MechBillWroker Props => (CompProperties_MechBillWroker)this.props;
    }
    public class CompProperties_MechBillWroker : CompProperties
    {
        public CompProperties_MechBillWroker() { this.compClass = typeof(CompMechBillWroker); }
    }
}
