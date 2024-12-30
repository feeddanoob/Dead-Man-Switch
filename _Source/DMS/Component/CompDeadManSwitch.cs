using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using System.Collections.Generic;
using Verse.Noise;
using System.Linq;
using RimWorld.Planet;
using System;
using static RimWorld.MechClusterSketch;

namespace DMS
{
    public class CompDeadManSwitch : ThingComp
    {
        public CompProperties_DeadManSwitch Props => (CompProperties_DeadManSwitch)props;
        public Pawn Overseer
        {
            get
            {
                Pawn pawn = (Pawn)this.parent;
                if (pawn == null)
                {
                    return null;
                }
                Pawn_RelationsTracker relations = pawn.relations;
                if (relations == null)
                {
                    return null;
                }
                return relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer, null);
            }
        }
        private int delayCheck;

        public bool woken;

        public bool woken_Lurk;
        public int timeToWake;
        public override void CompTick()
        {
            if (delayCheck > 0)
            {
                delayCheck--;
            }
            if (delayCheck <= 0)
            {
                //TryTriggerDMS();
                delayCheck = Props.minDelayUntilDMS;
            }

            if (!this.woken && this.woken_Lurk) 
            {
                if (this.timeToWake <= 0)
                {
                    this.Wake();        
                }
                this.timeToWake--;
            }
        }
        public void Wake()
        {
            this.woken = true;
            Pawn pawn = ((Pawn)this.parent);
            pawn.Name = new NameSingle(NameGenerator.GenerateName(this.Props.nameRule ?? RulePackDefOf.NamerTraderGeneral));

            Pawn_RelationsTracker relations = pawn.relations;
            if (relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer, null) is Pawn overseer)
            {
                pawn.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, overseer);
            }
            Find.LetterStack.ReceiveLetter("DMS_MechWake".Translate(this.parent.Label),
                "DMS_MechWakeDesc".Translate(this.parent.Label),LetterDefOf.PositiveEvent,this.parent);
            pawn.interactions = new Pawn_InteractionsTracker(pawn);
        }
        private void TryTriggerDMS()
        {
            Pawn mech = parent as Pawn;
            if (mech.IsWorldPawn() && mech.Faction != Faction.OfPlayer && mech.GetOverseer() != null)//媽的，又有機兵覺醒啦。
            {
                //得寫個生成讓他自己找回來
            }
            
            if (!mech.Faction.IsPlayer) return;
            CompOverseerSubject a = parent.GetComp<CompOverseerSubject>();

            var overseer = mech.GetOverseer()?.mechanitor;
            if (overseer != null) return;
            if (mech.Spawned && !overseer.CanControlMechs) //在無法連接時自己找其他人連接。
            {
                var li = mech.Map.mapPawns.FreeColonistsSpawned.Where(c => MechanitorUtility.IsMechanitor(c));
                if (li.Any())
                {
                    foreach (var c in li)
                    {
                        Log.Message(c.Name);
                        if (c.mechanitor.CanOverseeSubject(mech))
                        {
                            mech.GetOverseer()?.relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, mech);
                            c.relations.AddDirectRelation(PawnRelationDefOf.Overseer, mech);
                            Messages.Message("DMS_AutomatroidReconnected".Translate(), new LookTargets(parent), MessageTypeDefOf.PositiveEvent);
                            return;
                        }
                    }
                }
                Messages.Message("DMS_AutomatroidDisconnected".Translate(), new LookTargets(parent), MessageTypeDefOf.NeutralEvent);
                return;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            delayCheck = Props.minDelayUntilDMS;
        }
        public override string CompInspectStringExtra()
        {  
            if (!parent.EverSeenByPlayer || parent.GetComp<CompOverseerSubject>() == null) return null;
            if (parent.GetComp<CompOverseerSubject>().State != OverseerSubjectState.Overseen)
            {
                string str = "DMS_WillTerminateTheBetrayedUnit".Translate();
                return str;
            }
            return null;
        }
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            IThingHolder holder = this.parent.holdingOwner?.Owner;
            if (this.Overseer != null
                && (holder as Thing == null && holder as Caravan == null))
            {
                GameComponent_DMS dms = Current.Game.GetComponent<GameComponent_DMS>();
                dms.lostMechs.Add(this.parent);
            }
        }
        public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
        {
            base.Notify_Killed(prevMap, dinfo);
            this.woken = false;
            this.woken_Lurk = false;
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode && DebugSettings.godMode) 
            {
                yield return new Command_Action()
                {
                    defaultLabel = "Debug: Check State",
                    action = () => 
                    {
                        Log.Message(this.woken);
                        Log.Message(this.woken_Lurk);
                        Log.Message(this.timeToWake);
                    }
                };
                yield return new Command_Action()
                {
                    defaultLabel = "Debug: Wake",
                    action = () =>
                    {
                        this.Wake();
                    }
                };
            }
            yield break;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref delayCheck, "delayCheck", 0);
            Scribe_Values.Look(ref timeToWake, "timeToWake", 0);
            Scribe_Values.Look(ref woken_Lurk, "woken_Lurk");
            Scribe_Values.Look(ref woken, "woken");
        }


    }

    public class CompProperties_DeadManSwitch : CompProperties
    {
        public int minDelayUntilDMS = 3000;
        public float wakingChance= 0.5f;
        public RulePackDef nameRule;
        public CompProperties_DeadManSwitch()
        {
            compClass = typeof(CompDeadManSwitch);
        }
    }
}
