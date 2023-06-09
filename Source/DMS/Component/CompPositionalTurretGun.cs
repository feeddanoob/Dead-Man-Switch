using RimWorld;
using Verse;
using Verse.AI;

namespace DMS
{
    public class CompPositionalTurretGun : ThingComp , IAttackTargetSearcher
    {
        public Thing Thing => throw new System.NotImplementedException();

        public Verb CurrentEffectiveVerb => throw new System.NotImplementedException();

        public LocalTargetInfo LastAttackedTarget => throw new System.NotImplementedException();

        public int LastAttackTargetTick => throw new System.NotImplementedException();

        private CompProperties_PositionalTurretGun Props => (CompProperties_PositionalTurretGun)props;
    }
    public class CompProperties_PositionalTurretGun : CompProperties 
    {
        public ThingDef turretDef;

        public float angleOffset;

        public bool autoAttack = true;

        public CompProperties_PositionalTurretGun()
        {
            compClass = typeof(CompPositionalTurretGun);
        }
    }
}
