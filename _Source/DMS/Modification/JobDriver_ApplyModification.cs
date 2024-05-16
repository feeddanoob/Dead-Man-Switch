using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace DMS
{
    public class JobDriver_ApplyModification : JobDriver
    {
        private const int DurationTicks = 600;

        private Pawn Target => (Pawn)job.GetTarget(TargetIndex.A).Thing;

        private Thing Item => job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Target, job, 1, -1, null, errorOnFailed))
            {
                return pawn.Reserve(Item, job, 1, -1, null, errorOnFailed);
            }
            return false;
        }
        private Effecter effecter;
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.WaitWith(TargetIndex.A, DurationTicks,true,true);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.tickAction = delegate
            {
                if (effecter == null)
                {
                    effecter = (EffecterDefOf.MechRepairing).Spawn(Item, Item.Map);
                }
                effecter?.EffectTick(Item, Item);
            };
            yield return toil;
            yield return Toils_General.Do(ApplyModification);
        }

        private void ApplyModification()
        {
            Pawn p = Target;
            CompTargetable_AddHediffOnTarget comp = Item.TryGetComp<CompTargetable_AddHediffOnTarget>();
            SoundDefOf.MechSerumUsed.PlayOneShot(SoundInfo.InMap(p));
            Messages.Message("DMS_HasAppliedModification".Translate(p), p, MessageTypeDefOf.PositiveEvent);

            if (Target.health.hediffSet.TryGetHediff(comp.Props.hediffDef,out var h))
            {
                h.TryMergeWith(HediffMaker.MakeHediff(comp.Props.hediffDef, Target));
            }
            else Target.health.AddHediff(comp.Props.hediffDef);

            Item.SplitOff(1).Destroy();
        }
    }
}
