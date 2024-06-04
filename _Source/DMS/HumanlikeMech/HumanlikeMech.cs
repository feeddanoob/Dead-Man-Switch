using System.Collections.Generic;

using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace DMS
{
    public class HumanlikeMech : Pawn, IWeaponUsable
    {
        public HumanlikeMechExtension Extension { get; private set; }

        public Graphic_Multi HeadGraphic
        {
            get
            {
                if (cachedHeadGraphic == null)
                {
                    cachedHeadGraphic = this.Extension.headGraphic.Graphic as Graphic_Multi;
                }
                return cachedHeadGraphic; 
            }
        }

        public Graphic_Multi cachedHeadGraphic;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.Extension = this.def.GetModExtension<HumanlikeMechExtension>();
            if (Extension != null)
            {
                if (this.story != null)
                {
                    this.story.bodyType = Extension.bodyTypeOverride;
                    this.story.SkinColorBase = Color.white;
                }
                else
                {
                    this.story = new Pawn_StoryTracker(this);
                    this.story.bodyType = Extension.bodyTypeOverride;
                    this.story.SkinColorBase = Color.white;
                }
            }
        }
        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            if (this.Map == null) yield break;
            List<Thing> things = sq.GetThingList(this.Map);

            for (int i = 0; i < things.Count; i++)
            {
                ThingWithComps tmp = things[i] as ThingWithComps;
                if (tmp == null) continue;

                if (tmp.TryGetComp<CompEquippable>() != null)
                {
                    yield return this.TryMakeFloatMenuForWeapon(tmp);
                }
                if (tmp.def?.apparel != null)
                {
                    yield return this.TryMakeFloatMenuForApparel(tmp);
                }
            }
        }
        public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
        {
            base.DynamicDrawPhaseAt(phase, drawLoc, flip);
            this.DrawHeadOverride();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.Drawer?.renderer?.SetAllGraphicsDirty();
            }
        }
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
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
