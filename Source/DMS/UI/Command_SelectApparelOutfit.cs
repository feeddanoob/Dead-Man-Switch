﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;

namespace DMS
{
    public class Command_SelectApparelOutfit : Command_Action
    {
        public Pawn_OutfitTracker outfitSource;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => GetFloatMenu();

        private IEnumerable<FloatMenuOption> GetFloatMenu()
        {
            if (outfitSource == null) yield break;
            List<Outfit> outfis = Current.Game.outfitDatabase.AllOutfits;
            foreach (var outfit in outfis) {
                yield return new FloatMenuOption(outfit.label, () =>
                {
                    outfitSource.CurrentOutfit = outfit;
                    outfitSource.pawn.TryOptimizeApparel();
                });
            }
            yield break;
        }
    }
}
