using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    public class HediffComp_ProtectiveShield : HediffComp
    {
        public float DurablePercent => hitpoints / Props.hitpoints;
        private int hitpoints;
        private HediffCompProperties_ProtectiveShield Props
        {
            get
            {
                return (HediffCompProperties_ProtectiveShield)this.props;
            }
        }
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (hitpoints > 0)
            {
                hitpoints -= (int)totalDamageDealt;
                if (hitpoints < 0)
                {
                    base.Notify_PawnPostApplyDamage(dinfo, hitpoints);
                    hitpoints = 0;
                    Messages.Message("DMS_AddonBroken".Translate(), new LookTargets(parent.pawn.PositionHeld, parent.pawn.MapHeld), MessageTypeDefOf.NeutralEvent);
                    parent.pawn.health.RemoveHediff(parent);
                }
            }
            else
            {
                base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            }
        }
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref this.hitpoints, "hitpoints");
        }
    }
    public class HediffCompProperties_ProtectiveShield : HediffCompProperties
    {
        public int hitpoints;
        public HediffCompProperties_ProtectiveShield()
        {
            this.compClass = typeof(HediffComp_ProtectiveShield);
        }
    }
}
