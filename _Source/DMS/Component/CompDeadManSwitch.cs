using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using System.Collections.Generic;
using Verse.Noise;

namespace DMS
{
    //在失去機械師控制後只會自動關機
    public class CompDeadManSwitch : ThingComp
    {
        public CompProperties_DeadManSwitch Props => (CompProperties_DeadManSwitch)props;
        private int delayCheck;
        public override void CompTick()
        {
            if (delayCheck > 0)
            {
                delayCheck--;
            }
            if (delayCheck <= 0)
            {
                TryTriggerDMS();

                delayCheck = Props.minDelayUntilDMS;
            }
        }
        private void TryTriggerDMS()
        {
            var a = parent.GetComp<CompOverseerSubject>();
            if (((Pawn)a.parent).relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer) != null)
            {
                if (a?.State != OverseerSubjectState.Overseen)
                {
                    ((Pawn)a.parent).relations.RemoveDirectRelation(PawnRelationDefOf.Overseer, ((Pawn)parent)?.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Overseer));
                    Messages.Message("DMS_AutomatroidDisconnected".Translate(), new LookTargets(parent), MessageTypeDefOf.TaskCompletion);
                }
            }
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            Command_Action command_Action2 = new Command_Action();
            command_Action2.defaultLabel = "DEV: TriggerDMS";
            command_Action2.action = delegate
            {
                TryTriggerDMS();
            };
            yield return command_Action2;
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                delayCheck = Props.minDelayUntilDMS;
            }
        }
        public override string CompInspectStringExtra()
        {  
            if (parent.Faction != Faction.OfPlayer || parent.GetComp<CompOverseerSubject>() == null) return null;
            if (parent.GetComp<CompOverseerSubject>().State != OverseerSubjectState.Overseen)
            {
                string str = "DMS_WillTerminateTheBetrayedUnit".Translate();
                return str;
            }
            return null;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref delayCheck, "delayCheck", 0);
        }
    }

    public class CompProperties_DeadManSwitch : CompProperties
    {
        public int minDelayUntilDMS = 3000;

        public CompProperties_DeadManSwitch()
        {
            compClass = typeof(CompDeadManSwitch);
        }
    }
}
