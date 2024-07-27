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
                if (outfits == null)
                {
                    outfits = new Pawn_OutfitTracker(this);
                }
                if (story == null)
                    story = new Pawn_StoryTracker(this);
                story.bodyType = Extension.bodyTypeOverride;
                story.headType = Extension.headTypeOverride;
                story.SkinColorBase = Color.white;
            }
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
