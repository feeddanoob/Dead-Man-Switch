using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using static RimWorld.MechClusterSketch;

namespace DMS
{
    public class CompAbilityEffect_DropMech : CompAbilityEffect
    {
        public new CompProperties_DropMech Props => (CompProperties_DropMech)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            for (int i = 0; i < Props.spawnCount; i++)
            {
                List<Thing> list = new List<Thing>();
                Pawn thing = PawnGenerator.GeneratePawn(Props.KindDef, this.parent.pawn.Faction);
                if (CheckUtility.IsMech(thing) && CheckUtility.MechanitorCheck(this.parent.pawn.Map, out mechanitor))
                {
                    thing.relations.AddDirectRelation(PawnRelationDefOf.Overseer, mechanitor);
                }
                list.Add(thing);
                ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(list);
                DropPodUtility.MakeDropPodAt(target.Cell + Rand(3), parent.pawn.Map, activeDropPodInfo);
            }
            if (Props.sendSkipSignal)
            {
                CompAbilityEffect_Teleport.SendSkipUsedSignal(target, parent.pawn);
            }
        }
        private IntVec3 Rand(int range)
        {
            return new IntVec3(Random.Range(-range, range), 0, Random.Range(-range, range));
        }
        private Pawn mechanitor;
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (target.Cell.Filled(parent.pawn.Map) || (!Props.allowOnBuildings && target.Cell.GetEdifice(parent.pawn.Map) != null))
            {
                if (throwMessages)
                {
                    Messages.Message("CannotUseAbility".Translate(parent.def.label) + ": " + "AbilityOccupiedCells".Translate(), target.ToTargetInfo(parent.pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
                }

                return false;
            }
            if (CheckUtility.IsMech(parent.pawn))
            {
                if (CheckUtility.MechanitorCheck(parent.pawn.Map, out Pawn pawn))
                {
                    mechanitor = pawn;
                }
                else
                {
                    Messages.Message("CommandCallRoyalAid_NoMechanitorAvaliable".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }
            }
            return true;
        }
    }

    public class CompProperties_DropMech : CompProperties_AbilityEffect
    {
        public PawnKindDef KindDef;

        public bool allowOnBuildings = true;

        public bool sendSkipSignal = true;

        public int spawnCount = 1;

        public CompProperties_DropMech()
        {
            compClass = typeof(CompAbilityEffect_DropMech);
        }
    }
}
