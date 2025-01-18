using Mono.Unix.Native;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.AI;
using Verse;

namespace DMS
{
    public class PawnRenderNode_ApparelTurretGun : PawnRenderNode
    {
        public CompApparelTurretGun turretComp;

        public PawnRenderNode_ApparelTurretGun(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            turretComp ??= this.parent.apparel.GetComp<CompApparelTurretGun>();
            return GraphicDatabase.Get<Graphic_Single>(turretComp.Props.turretDef.graphicData.texPath, ShaderDatabase.Cutout);
        }
    }
    public class PawnRenderNodeWorker_ApparelTurretGun : PawnRenderNodeWorker
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            if (!base.CanDrawNow(node, parms))
            {
                return false;
            }
            PawnRenderNode_ApparelTurretGun pawnRenderNode = node as PawnRenderNode_ApparelTurretGun;
            pawnRenderNode.turretComp ??= pawnRenderNode.apparel.GetComp<CompApparelTurretGun>();
            if (pawnRenderNode.turretComp != null)
            {
                Log.Message("X");
                return true;
            }
            return false;
        }

        public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Quaternion result = base.RotationFor(node, parms);
            PawnRenderNode_ApparelTurretGun pawnRenderNode = node as PawnRenderNode_ApparelTurretGun;

            pawnRenderNode.turretComp = pawnRenderNode.apparel.TryGetComp<CompApparelTurretGun>();
            Pawn pawnOwner = pawnRenderNode.turretComp.PawnOwner;
            if (pawnRenderNode != null)
            {
                if (pawnRenderNode.turretComp.currentTarget == LocalTargetInfo.Invalid && pawnOwner.CurJob != null && pawnOwner.CurJob.targetA != null)
                {
                    IntVec3 position = pawnOwner.Position;
                    IntVec3 cell = pawnOwner.CurJob.targetA.Cell;
                    float ang = (cell - position).ToVector3Shifted().ToAngleFlat();
                    result *= ang.ToQuat();
                }
                result *= pawnRenderNode.turretComp.curRotation.ToQuat();
            }
            return result;
        }

        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            PawnRenderNode_ApparelTurretGun pawnRenderNode = node as PawnRenderNode_ApparelTurretGun;
            pawnRenderNode.turretComp = pawnRenderNode.apparel.TryGetComp<CompApparelTurretGun>();
            if (pawnRenderNode != null && pawnRenderNode.turretComp != null)
            {
                Vector3 vector = node.Props.drawData.OffsetForRot(parms.facing);
                return base.OffsetFor(node, parms, out pivot) + vector;
            }
            return base.OffsetFor(node, parms, out pivot);
        }
    }

    public class CompProperties_ApparelTurretGun : CompProperties
    {
        public ThingDef turretDef;

        public float angleOffset;

        public bool autoAttack = true;

        public bool attackUndrafted = true;

        public List<PawnRenderNodeProperties> renderNodeProperties;

        public CompProperties_ApparelTurretGun()
        {
            compClass = typeof(CompApparelTurretGun);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (renderNodeProperties.NullOrEmpty())
            {
                yield break;
            }
            foreach (PawnRenderNodeProperties renderNodeProperty in renderNodeProperties)
            {
                if (!typeof(PawnRenderNode_ApparelTurretGun).IsAssignableFrom(renderNodeProperty.nodeClass))
                {
                    yield return "contains unsupported nodeClass.";
                }
            }
        }
    }

    public class CompApparelTurretGun : ThingComp, IAttackTargetSearcher
    {
        private static readonly CachedTexture ToggleTurretIcon = new CachedTexture("UI/Gizmos/ToggleTurret");

        public Thing gun;

        protected int burstCooldownTicksLeft;

        protected int burstWarmupTicksLeft;

        public LocalTargetInfo currentTarget = LocalTargetInfo.Invalid;

        private bool fireAtWill = true;

        private LocalTargetInfo lastAttackedTarget = LocalTargetInfo.Invalid;

        private int lastAttackTargetTick;

        public float curRotation;

        public Thing Thing => PawnOwner;

        public CompProperties_ApparelTurretGun Props => (CompProperties_ApparelTurretGun)props;

        public Verb CurrentEffectiveVerb => AttackVerb;

        public LocalTargetInfo LastAttackedTarget => lastAttackedTarget;

        public int LastAttackTargetTick => lastAttackTargetTick;

        public CompEquippable GunCompEq => gun.TryGetComp<CompEquippable>();

        public Verb AttackVerb => GunCompEq.PrimaryVerb;

        private bool WarmingUp => burstWarmupTicksLeft > 0;

        public Pawn PawnOwner
        {
            get
            {
                if (!(parent is Apparel { Wearer: var wearer }))
                {
                    if (parent is Pawn result)
                    {
                        return result;
                    }
                    return null;
                }
                return wearer;
            }
        }
        public bool CanShoot
        {
            get
            {
                if (PawnOwner != null)
                {
                    if (!PawnOwner.Spawned || PawnOwner.Downed || PawnOwner.Dead || !PawnOwner.Awake())
                    {
                        return false;
                    }
                    if (!Props.attackUndrafted && PawnOwner.IsPlayerControlled && !PawnOwner.Drafted)
                    {
                        if (!fireAtWill)
                        {
                            return false;
                        }
                    }
                    if (PawnOwner.stances.stunner.Stunned)
                    {
                        return false;
                    }
                    if (!fireAtWill)
                    {
                        return false;
                    }
                    CompCanBeDormant compCanBeDormant = PawnOwner.TryGetComp<CompCanBeDormant>();
                    if (compCanBeDormant != null && !compCanBeDormant.Awake)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public bool AutoAttack => Props.autoAttack;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if ((respawningAfterLoad))
            {
                MakeGun();
            }
        }
        public override void Notify_Equipped(Pawn pawn)
        {
            MakeGun();
        }

        private void MakeGun()
        {
            gun = ThingMaker.MakeThing(Props.turretDef);
            UpdateGunVerbs();
        }

        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = PawnOwner;
                verb.castCompleteCallback = delegate
                {
                    burstCooldownTicksLeft = AttackVerb.verbProps.defaultCooldownTime.SecondsToTicks();
                };
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!CanShoot)
            {
                return;
            }
            if (currentTarget.IsValid)
            {
                curRotation = (currentTarget.Cell.ToVector3Shifted() - PawnOwner.DrawPos).AngleFlat() + Props.angleOffset;
            }
            AttackVerb.VerbTick();
            if (AttackVerb.state == VerbState.Bursting)
            {
                return;
            }
            if (WarmingUp)
            {
                burstWarmupTicksLeft--;
                if (burstWarmupTicksLeft == 0)
                {
                    AttackVerb.TryStartCastOn(currentTarget, surpriseAttack: false, canHitNonTargetPawns: true, preventFriendlyFire: false, nonInterruptingSelfCast: true);
                    lastAttackTargetTick = Find.TickManager.TicksGame;
                    lastAttackedTarget = currentTarget;
                }
                return;
            }
            if (burstCooldownTicksLeft > 0)
            {
                burstCooldownTicksLeft--;
            }
            if (burstCooldownTicksLeft <= 0 && PawnOwner.IsHashIntervalTick(10))
            {
                currentTarget = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(this, TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable);
                if (currentTarget.IsValid)
                {
                    burstWarmupTicksLeft = 1;
                }
                else
                {
                    ResetCurrentTarget();
                }
            }
        }

        private void ResetCurrentTarget()
        {
            currentTarget = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetWornGizmosExtra())
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
            if (PawnOwner.Faction == Faction.OfPlayer && (PawnOwner.Drafted))
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "CommandToggleTurret".Translate(),
                    defaultDesc = "CommandToggleTurretDesc".Translate(),
                    isActive = () => fireAtWill,
                    icon = ToggleTurretIcon.Texture,
                    toggleAction = delegate
                    {
                        fireAtWill = !fireAtWill;
                        PawnOwner.Drawer.renderer.renderTree.SetDirty();
                    }
                };
            }
        }

        public override List<PawnRenderNode> CompRenderNodes()
        {
            if (!Props.renderNodeProperties.NullOrEmpty() && PawnOwner != null)
            {
                List<PawnRenderNode> list = new List<PawnRenderNode>();
                foreach (PawnRenderNodeProperties renderNodeProperty in Props.renderNodeProperties)
                {
                    PawnRenderNode_ApparelTurretGun pawnRenderNode = 
                        new PawnRenderNode_ApparelTurretGun(PawnOwner,renderNodeProperty, PawnOwner.Drawer.renderer.renderTree)
                        {
                        turretComp = this,
                        apparel = parent as Apparel
                        };
                    list.Add(pawnRenderNode);
                }
                return list;
            }
            return base.CompRenderNodes();
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (Props.turretDef != null)
            {

                yield return new StatDrawEntry(StatCategoryDefOf.PawnCombat, "DMS_TurretPlatform".Translate(), Props.turretDef.LabelCap, "DMS_TurretPlatformDesc".Translate(), 5600, null, Gen.YieldSingle(new Dialog_InfoCard.Hyperlink(Props.turretDef)));
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref burstCooldownTicksLeft, "burstCooldownTicksLeft", 0);
            Scribe_Values.Look(ref burstWarmupTicksLeft, "burstWarmupTicksLeft", 0);
            Scribe_TargetInfo.Look(ref currentTarget, "currentTarget");
            Scribe_Deep.Look(ref gun, "gun");
            Scribe_Values.Look(ref fireAtWill, "fireAtWill", defaultValue: true);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (gun == null && PawnOwner != null)
                {
                    Log.Error("CompTurrentGun had null gun after loading. Recreating.");
                    MakeGun();
                }
                else
                {
                    UpdateGunVerbs();
                }
            }
        }
    }
}