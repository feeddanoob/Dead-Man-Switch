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
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
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
