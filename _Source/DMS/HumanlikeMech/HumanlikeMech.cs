using System.Collections.Generic;

using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using System.Linq;
using RimWorld.Planet;
using static RimWorld.MechClusterSketch;

namespace DMS
{
    public class HumanlikeMech : Pawn, IWeaponUsable
    {
        private Graphic headGraphic;
        public MechWeaponExtension MechWeapon { get; private set; }
        public HumanlikeMechExtension Extension { get; private set; }
        public Graphic HeadGraphic
        {
            get
            {
                if (headGraphic == null)
                    headGraphic = Extension.headGraphic.Graphic;
                return headGraphic;
            }
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            MechWeapon = def.GetModExtension<MechWeaponExtension>();
            Extension = def.GetModExtension<HumanlikeMechExtension>();
            if (Extension != null)
            {
                outfits ??= new Pawn_OutfitTracker(this);
                story ??= new Pawn_StoryTracker(this)
                {
                    bodyType = Extension.bodyTypeOverride,
                    headType = Extension.headTypeOverride,
                    SkinColorBase = Color.white
                };

                interactions ??= new(this);
                skills ??= new(this);
                skills.skills.ForEach(s => s.Level = def.race.mechFixedSkillLevel);
            }
        }
        public override void Kill(DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (dinfo == null)//解體殺
            {
                List<Hediff> hediffs = health.hediffSet.hediffs.Where(h => h.def.spawnThingOnRemoved != null).ToList();
                foreach (Hediff item in hediffs)
                {
                    health.RemoveHediff(item);
                    Thing thing = ThingMaker.MakeThing(item.def.spawnThingOnRemoved);
                    thing.stackCount = 1;
                    GenPlace.TryPlaceThing(thing, this.Position, this.Map, ThingPlaceMode.Near);
                }
            }
            base.Kill(dinfo, exactCulprit);
        }
        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            if (IsColonyMechPlayerControlled)
            {
                foreach (var item in base.GetExtraFloatMenuOptionsFor(sq))
                {
                    yield return item;
                }
                foreach (var item in FloatMenuUtility.GetExtraFloatMenuOptionsFor(this, sq, MechWeapon))
                {
                    yield return item;
                }
                if (this.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch comp && comp.woken && sq == this.Position
                    && MechRepairUtility.CanRepair(this))
                {
                    yield return new FloatMenuOption("RepairMech".Translate(this.LabelShort), () =>
                    {
                        Job job = JobMaker.MakeJob(DMS_DefOf.DMS_RepairSelf, this);
                        this.jobs.StartJob(job);
                    });
                }
                yield break;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.Drawer?.renderer?.SetAllGraphicsDirty();
            }
        }
        public void Equip(ThingWithComps equipment)
        {
            equipment.SetForbidden(false);
            this.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Equip, equipment), JobTag.Misc);
        }

        public void Wear(ThingWithComps apparel)
        {
            apparel.SetForbidden(false);
            this.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Wear, apparel), JobTag.Misc);
        }


    }
}
