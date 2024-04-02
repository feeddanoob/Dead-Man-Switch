﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DMS
{
    public class CompMultipleTurretGun : ThingComp
    {
        public CompPropertiesMultipleTurretGun Props
        {
            get
            {
                return (CompPropertiesMultipleTurretGun)this.props;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.Props.subTurrets.ForEach(t =>
            {
                SubTurret turret = new SubTurret() { ID = t.ID, parent = this.parent };
                turret.Init(t);
                this.turrets.Add(turret);
            });
        }
        public override void CompTick()
        {
            base.CompTick();
            this.turrets.ForEach(t => t.Tick());
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (this.parent is Pawn pawn && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                foreach (SubTurret t in this.turrets)
                {
                    foreach (Gizmo g in t.GetGizmo())
                    {
                        yield return g;
                    }
                }
            }
            yield break;
        }
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (!this.Props.subTurrets.NullOrEmpty())
            {
                foreach (SubTurretProperties t in this.Props.subTurrets)
                {
                    yield return new StatDrawEntry(StatCategoryDefOf.PawnCombat, "Turret".Translate(),t.turret.LabelCap, "Stat_Thing_TurretDesc".Translate(), 5600, null, Gen.YieldSingle<Dialog_InfoCard.Hyperlink>(new Dialog_InfoCard.Hyperlink(t.turret, -1)), false, false);
                }
            }
            yield break;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref this.turrets, "turrets",LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.turrets.ForEach(t =>
                {
                    t.Init(this.Props.subTurrets.Find(t2 => t2.ID == t.ID));
                });
            }
        }

        public override List<PawnRenderNode> CompRenderNodes()
        {
            if (this.parent is Pawn pawn)
            {
                List<PawnRenderNode> list = new List<PawnRenderNode>();

                foreach (SubTurret t in this.turrets)
                {
                    list.AddRange(t.RenderNodes(pawn));
                }
                return list;
            }
            return base.CompRenderNodes();
        }

        public List<SubTurret> turrets = new List<SubTurret>(); 
    }
    [StaticConstructorOnStartup]
    public class SubTurret : IAttackTargetSearcher,IExposable
    {
        public Thing Thing => this.parent;

        public Verb CurrentEffectiveVerb => this.GunCompEq.PrimaryVerb;
        public CompEquippable GunCompEq
        {
            get
            {
                return this.turret.TryGetComp<CompEquippable>();
            }
        }

        public LocalTargetInfo LastAttackedTarget => this.lastAttackedTarget;

        public int LastAttackTargetTick => this.lastAttackTargetTick; 
        private bool CanShoot
        {
            get
            {
                Pawn pawn;
                if ((pawn = (this.parent as Pawn)) != null)
                {
                    if (!pawn.Spawned || pawn.Downed || pawn.Dead || !pawn.Awake())
                    {
                        return false;
                    }
                    if (pawn.stances.stunner.Stunned)
                    {
                        return false;
                    }
                    if (this.TurretDestroyed)
                    {
                        return false;
                    }
                    if (pawn.IsColonyMechPlayerControlled && !this.fireAtWill)
                    {
                        return false;
                    }
                }
                CompCanBeDormant compCanBeDormant = this.parent.TryGetComp<CompCanBeDormant>();
                return compCanBeDormant == null || compCanBeDormant.Awake;
            }
        }
        private bool WarmingUp
        {
            get
            {
                return this.burstWarmupTicksLeft > 0;
            }
        }
        public bool TurretDestroyed
        {
            get
            {
                Pawn pawn;
                return (pawn = (this.parent as Pawn)) != null && this.CurrentEffectiveVerb.verbProps.linkedBodyPartsGroup != null && this.CurrentEffectiveVerb.verbProps.ensureLinkedBodyPartsGroupAlwaysUsable && PawnCapacityUtility.CalculateNaturalPartsAverageEfficiency(pawn.health.hediffSet, this.CurrentEffectiveVerb.verbProps.linkedBodyPartsGroup) <= 0f;
            }
        }
        public void Init(SubTurretProperties prop)
        {   
            this.turretProp = prop;
            if (this.turret == null)
            {
                this.turret = ThingMaker.MakeThing(this.turretProp.turret, null);
            }
            this.UpdateGunVerbs();
        }
        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = this.turret.TryGetComp<CompEquippable>().AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = this.parent;
                verb.castCompleteCallback = delegate ()
                {
                    this.burstCooldownTicksLeft = this.CurrentEffectiveVerb.verbProps.defaultCooldownTime.SecondsToTicks();
                };
            }
        }
        public void Tick()
        {
            if (!this.CanShoot)
            {
                return;
            }
            if (this.currentTarget.IsValid)
            {
                this.curRotation = (this.currentTarget.Cell.ToVector3Shifted() - this.parent.DrawPos).AngleFlat() + this.turretProp.IdleAngleOffset;
            }
            this.CurrentEffectiveVerb.VerbTick();
            if (this.CurrentEffectiveVerb.state != VerbState.Bursting)
            {
                if (this.WarmingUp)
                {
                    this.burstWarmupTicksLeft--;
                    if (this.burstWarmupTicksLeft == 0)
                    {
                        this.CurrentEffectiveVerb.TryStartCastOn(this.currentTarget, false, true, false, true);
                        this.lastAttackTargetTick = Find.TickManager.TicksGame;
                        this.lastAttackedTarget = this.currentTarget;
                        return;
                    }
                }
                else
                {
                    if (this.burstCooldownTicksLeft > 0)
                    {
                        this.burstCooldownTicksLeft--;
                    }
                    if (this.burstCooldownTicksLeft <= 0 && this.parent.IsHashIntervalTick(10))
                    {
                        if (this.turretProp.autoAttack)
                        {
                            this.currentTarget = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(this, TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable, null, 0f, 9999f);
                        }
                        if (this.currentTarget.IsValid)
                        {
                            this.burstWarmupTicksLeft = 1;
                            return;
                        }
                        this.ResetCurrentTarget();
                    }
                }
            }
        }
        private void ResetCurrentTarget()
        {
            this.currentTarget = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
        }
        public List<PawnRenderNode> RenderNodes(Pawn pawn) 
        {
            List<PawnRenderNode> result = new List<PawnRenderNode>();
            this.turretProp.renderNodeProperties.ForEach(p => 
            {
                PawnRenderNode_SubTurretGun pawnRenderNode_TurretGun = (PawnRenderNode_SubTurretGun)Activator.CreateInstance(p.nodeClass, new object[]
{
                        pawn,
                        p,
                        pawn.Drawer.renderer.renderTree
});
                pawnRenderNode_TurretGun.subturret = this;
                result.Add(pawnRenderNode_TurretGun);
            });
            return result;
        }
        public IEnumerable<Gizmo> GetGizmo()
        {
            yield return new Command_Toggle
            {
                defaultLabel = this.turretProp.command_ToggleFire.Translate(this.turret.def.label),
                defaultDesc = this.turretProp.commandDesc_ToggleFire.Translate(),
                isActive = (() => this.fireAtWill),
                icon = SubTurret.ToggleTurretIcon.Texture,
                toggleAction = delegate ()
                {
                    this.fireAtWill = !this.fireAtWill;
                }
            };
            if (!this.turretProp.autoAttack)
            {
                yield return new Command_Action
                {
                    defaultLabel = this.turretProp.command_Target.Translate(this.turret.def.label),
                    defaultDesc = this.turretProp.commandDesc_Target.Translate(),
                    icon = ContentFinder<Texture2D>.Get(this.turretProp.targetCommandIconPath),
                    action = delegate ()
                    {
                        Find.Targeter.BeginTargeting(this.CurrentEffectiveVerb.targetParams, t => this.currentTarget = t);
                    }
                };
            }
            yield break;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.ID, "ID");
            Scribe_Values.Look<int>(ref this.burstCooldownTicksLeft, "burstCooldownTicksLeft", 0, false);
            Scribe_Values.Look<int>(ref this.burstWarmupTicksLeft, "burstWarmupTicksLeft", 0, false);
            Scribe_TargetInfo.Look(ref this.currentTarget, "currentTarget");
            Scribe_Values.Look<bool>(ref this.fireAtWill, "fireAtWill", true, false);
            Scribe_References.Look(ref this.parent, "parent");
            Scribe_Deep.Look(ref this.turret, "turret");
        }

        public string ID = "null";

        public Thing parent;
        public Thing turret;

        private static readonly CachedTexture ToggleTurretIcon = new CachedTexture("UI/Gizmos/ToggleTurret");

        protected int burstCooldownTicksLeft;
        protected int burstWarmupTicksLeft;
        protected LocalTargetInfo currentTarget = LocalTargetInfo.Invalid;

        private bool fireAtWill = true;
        private LocalTargetInfo lastAttackedTarget = LocalTargetInfo.Invalid;
        private int lastAttackTargetTick;
        public SubTurretProperties turretProp;

        public float curRotation;
    }
}