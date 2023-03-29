﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace DMS
{
    public class HumanlikeMech : Pawn
    {
        public HumanlikeMechExtension Extension { get; private set; }
        public Graphic_Multi HeadGraphic
        {
            get
            {
                if(cachedHeadGraphic == null)
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

                if(tmp.def?.apparel != null)
                {
                    
                    yield return this.TryMakeFloatMenuForApparel(tmp);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            this.DrawHeadOverride();
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