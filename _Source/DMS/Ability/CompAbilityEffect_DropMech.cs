﻿using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DMS
{

    public class CompAbilityEffect_DropMech : CompAbilityEffect
    {
        public new CompProperties_DropMech Props => (CompProperties_DropMech)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (parent.def.GetModExtension<PawnKindExtension>() != null)
            {
                foreach (Member m in parent.def.GetModExtension<PawnKindExtension>().members)
                {
                    List<Thing> list = new List<Thing>();
                    Pawn pawn = PawnGenerator.GeneratePawn(m.pawnKind, this.parent.pawn.Faction);
                    if (m.fixedWeapon != null)
                    {
                        pawn.equipment.Remove(pawn.equipment.Primary);
                        Thing weapon = ThingMaker.MakeThing(m.fixedWeapon);
                        pawn.equipment.AddEquipment(weapon as ThingWithComps);
                    }
                    if (!m.additionalThings.NullOrEmpty()) 
                    {
                        foreach (var thing in m.additionalThings)
                        {
                            Thing item = ThingMaker.MakeThing(thing.ThingDef);
                            item.stackCount = thing.Count;
                            pawn.inventory.TryAddItemNotForSale(item);
                        }
                    }
                    if (CheckUtility.IsMech(pawn) && pawn.Faction.IsPlayer)
                    {
                        pawn.relations.AddDirectRelation(PawnRelationDefOf.Overseer, this.parent.pawn.GetOverseer().mechanitor.Pawn);
                    }
                    list.Add(pawn);
                    ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                    activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(list,false);
                    DropPodUtility.MakeDropPodAt(target.Cell + Rand(3), parent.pawn.Map, activeDropPodInfo);
                }
            }
            else
            {
                for (int i = 0; i < Props.spawnCount; i++)
                {
                    List<Thing> list = new List<Thing>();
                    {
                        Pawn pawn = PawnGenerator.GeneratePawn(Props.KindDef, this.parent.pawn.Faction);
                        if (CheckUtility.IsMech(pawn) && CheckUtility.MechanitorCheck(this.parent.pawn.Map, out mechanitor))
                        {
                            pawn.relations.AddDirectRelation(PawnRelationDefOf.Overseer, this.parent.pawn.GetOverseer().mechanitor.Pawn);
                        }
                        list.Add(pawn);
                    }

                    ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                    activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(list);
                    DropPodUtility.MakeDropPodAt(target.Cell + Rand(3), parent.pawn.Map, activeDropPodInfo);
                }
            }
            
            if (Props.sendSkipSignal)
            {
                CompAbilityEffect_Teleport.SendSkipUsedSignal(target, parent.pawn);
            }
        }
        private IntVec3 Rand(int range)
        {
            return new IntVec3(Verse.Rand.Range(-range, range), 0, Verse.Rand.Range(-range, range));
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
