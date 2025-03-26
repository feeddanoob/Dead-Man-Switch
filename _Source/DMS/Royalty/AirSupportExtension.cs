using Verse;

namespace DMS
{
    public class AirSupportExtension : DefModExtension
    {
        public IntRange delayRange = new IntRange(120, 150);

        public int preEffectTick = 0;
        public EffecterDef preSupportEffecterDef;

        public ThingDef projectileDef = null;
        public float spreadRadius = 5;
        public int burstCount = 1, burstInterval = 5;

        public int postEffectTick = 0;
        public EffecterDef postSupportEffecterDef;
    }
}