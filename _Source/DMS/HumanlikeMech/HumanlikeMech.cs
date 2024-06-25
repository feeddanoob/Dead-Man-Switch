using System.Collections.Generic;

using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace DMS
{
    public class HumanlikeMech : Pawn, IWeaponUsable
    {
        private Graphic headGraphic;

        public HumanlikeMechExtension Extension { get; private set; }
        public Graphic HeadGraphic { get {
                if (headGraphic == null)
                    headGraphic = Extension.headGraphic.Graphic;
                return headGraphic;
            } 
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Extension = def.GetModExtension<HumanlikeMechExtension>();
            if (Extension != null)
            {
                if (outfits == null)
                {
                    outfits = new Pawn_OutfitTracker(this);
                }
                if (story==null)
                story = new Pawn_StoryTracker(this);
                story.bodyType = Extension.bodyTypeOverride;
                story.headType = Extension.headTypeOverride;
                story.SkinColorBase = Color.white;
            }
        }
        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            if (Map == null || !IsColonyMech) yield break;
            List<Thing> things = sq.GetThingList(this.Map);

            for (int i = 0; i < things.Count; i++)
            {
                if (!(things[i] is ThingWithComps tmp)) continue;

                if (tmp.TryGetComp<CompEquippable>() != null)
                {
                    yield return this.TryMakeFloatMenuForWeapon(tmp);
                }
                if (tmp.def.IsApparel)
                {
                    yield return this.TryMakeFloatMenuForApparel(tmp);
                }
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
