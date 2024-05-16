using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DMS
{
    public class HediffComp_ProtectiveShield : HediffComp
    {
        public float DurablePercent => hitpoints / Props.hitpoints;
        public float Hitpoints => hitpoints;
        private int hitpoints;
        public HediffCompProperties_ProtectiveShield Props
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
        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            foreach (Gizmo item in base.CompGetGizmos())
            {
                yield return item;
            }
            foreach (Gizmo gizmo in GetGizmos())
            {
                yield return gizmo;
            }
        }
        private IEnumerable<Gizmo> GetGizmos()
        {
            if ((parent.pawn.Faction == Faction.OfPlayer || (parent.pawn.RaceProps.IsMechanoid)) && Find.Selector.SingleSelectedThing == parent.pawn)
            {
                Gizmo_AttachmentShieldStatus gizmo_Shield = new Gizmo_AttachmentShieldStatus
                {
                    shield = this
                };
                yield return gizmo_Shield;
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

    [StaticConstructorOnStartup]
    public class Gizmo_AttachmentShieldStatus : Gizmo
    {
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public HediffComp_ProtectiveShield shield;
        public Gizmo_AttachmentShieldStatus()
        {
            Order = -120f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, shield.parent.LabelCap);
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = shield.Hitpoints / Mathf.Max(1f, shield.Props.hitpoints);
            Widgets.FillableBar(rect4, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, (shield.Hitpoints * 100f).ToString("F0") + " / " + shield.Props.hitpoints);
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect2, "ShieldPersonalTip".Translate());
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
