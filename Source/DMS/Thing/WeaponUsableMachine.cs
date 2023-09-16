using Verse;
using RimWorld;
using Verse.AI;
using VFE.Mechanoids;

namespace DMS
{
    //給VFEM機械使用的
    public class WeaponUsableMachine : WeaponUsableMech
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            mindState.lastJobTag = JobTag.Idle;
            guest = new Pawn_GuestTracker(this);
            if (drafter == null && this.TryGetComp<CompMachine>().Props.violent)
            {
                drafter = new Pawn_DraftController(this);
            }

            if (base.Faction == Faction.OfPlayer && base.Name == null)
            {
                base.Name = PawnBioAndNameGenerator.GeneratePawnName(this, NameStyle.Numeric);
            }
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            if (!health.Dead && (!health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || !health.capacities.CapableOf(PawnCapacityDefOf.Moving)))
            {
                Kill(dinfo);
            }
        }
    }
}
