using Verse;

namespace DMS
{
    public abstract class AirSupportComp
    {
        public abstract void Trigger(AirSupportDef def, Thing trigger, Map map, LocalTargetInfo target);
    }
}
