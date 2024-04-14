using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DMS
{
    public class CompAbilityEffect_SelfRepairMode : CompAbilityEffect
    {
        public new CompProperties_AbilitySelfRepairMode Props => (CompProperties_AbilitySelfRepairMode)props;
        public override bool CanCast => base.CanCast && IsInjuredAndAlive() && parent.pawn.IsPlayerControlled;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if (pawn == null) return;

            List<Hediff> hediffs = (from Hediff item in target.Pawn.health.hediffSet.hediffs.Where(p => p is Hediff_MissingPart) select item).ToList();
            if (hediffs.NullOrEmpty()) return;

            foreach (var item in hediffs)
            {
                float dmg = Rand.Range(item.Part.def.GetMaxHealth(target.Pawn) / 2, item.Part.def.GetMaxHealth(target.Pawn) - 1);
                target.Pawn.health.RemoveHediff(item);
                DamageInfo damage = new DamageInfo(DamageDefOf.ElectricalBurn, dmg, 999, -1, null, item.Part);
                target.Pawn.TakeDamage(damage);
            }
        }
        private bool IsInjuredAndAlive()
        {
            if (parent.pawn.Spawned && parent.pawn != null && !parent.pawn.Dead)
            {
                return true;
            }
            return false;
        }
    }
    public class CompProperties_AbilitySelfRepairMode : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySelfRepairMode()
        {
            compClass = typeof(CompAbilityEffect_SelfRepairMode);
        }
    }
}
