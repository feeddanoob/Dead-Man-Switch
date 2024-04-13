using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DMS
{
    public class JobDriver_SelfHealing : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return ReservationUtility.Reserve(this.pawn, this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            ToilFailConditions.FailOnDespawnedOrNull(this, (TargetIndex)1);
            Toil WaitToHeal = ToilMaker.MakeToil("WaitToHeal");
            WaitToHeal.initAction = delegate ()
            {
                WaitToHeal.actor.pather.StopDead();
            };
            WaitToHeal.defaultCompleteMode = (ToilCompleteMode)3;
            WaitToHeal.defaultDuration = 600;
            WaitToHeal.tickAction = delegate ()
            {
                this.FleckTick++;
                bool flag = this.FleckTick >= this.FleckTickMax;
                if (flag)
                {
                    this.FleckTick = 0;
                    Map map = this.pawn.Map;
                    FleckCreationData dataStatic = FleckMaker.GetDataStatic(this.pawn.DrawPos, map, FleckDefOf.HealingCross, 1f);
                    float velocityAngle = Rand.Range(-30f, 30f);
                    dataStatic.velocityAngle = velocityAngle;
                    dataStatic.velocitySpeed = Rand.Range(0.5f, 1f);
                    map.flecks.CreateFleck(dataStatic);
                }
            };
            ToilEffects.WithProgressBarToilDelay(WaitToHeal, (TargetIndex)1, false, -0.5f);
            yield return WaitToHeal;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    bool flag = false;
                    foreach (Hediff hediff in this.pawn.health.hediffSet.hediffs)
                    {
                        bool flag2 = hediff is Hediff_Injury && !HediffUtility.IsPermanent(hediff);
                        if (flag2)
                        {
                            flag = true;
                            break;
                        }
                    }
                    bool flag3 = flag;
                    if (flag3)
                    {
                        foreach (Hediff hediff2 in this.pawn.health.hediffSet.hediffs)
                        {
                            Hediff_Injury hediff_Injury = (Hediff_Injury)hediff2;
                            bool flag4 = HediffUtility.CanHealNaturally(hediff_Injury) || HediffUtility.CanHealFromTending(hediff_Injury);
                            if (flag4)
                            {
                                hediff_Injury.Heal(this.HealNum);
                                Vector3 drawPos = this.pawn.DrawPos;
                                Map map = this.pawn.Map;
                                FleckMaker.ThrowSmoke(drawPos, map, 1.5f);
                                FleckMaker.ThrowMicroSparks(drawPos, map);
                                FleckMaker.ThrowLightningGlow(drawPos, map, 1.5f);
                                this.pawn.jobs.EndCurrentJob((JobCondition)2, true, true);
                                Job job = JobMaker.MakeJob(DMS_JobDefOf.DMS_SelfRepair, this.pawn);
                                job.count = 1;
                                this.pawn.jobs.TryTakeOrderedJob(job, new JobTag?(0), false);
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.pawn.records.Increment(RecordDefOf.ThingsRepaired);
                        this.pawn.jobs.EndCurrentJob((JobCondition)2, true, true);
                        this.pawn.stances.SetStance(new Stance_Cooldown(30, this.pawn, null));
                    }
                },
                defaultCompleteMode = (ToilCompleteMode)1
            };
            yield break;
        }

        public float HealNum = 5f;

        public int FleckTick;

        public int FleckTickMax = 60;
    }
    [DefOf]
    public static class DMS_JobDefOf
    {
        public static JobDef DMS_SelfRepair;
    }
}
