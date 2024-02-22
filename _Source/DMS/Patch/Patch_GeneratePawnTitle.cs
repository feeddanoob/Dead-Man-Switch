using Verse;
using RimWorld;
using HarmonyLib;
using System;

namespace DMS
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(new Type[] {typeof(PawnGenerationRequest)})]
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch(nameof(PawnGenerator.GeneratePawn))]

    internal static class Patch_GeneratePawnTitle
    {
        public static void Postfix(ref Pawn __result)
        {
            if (__result.kindDef.HasModExtension<DefaultTilteFactionExtension>())
            {
                Faction faction = Find.FactionManager?.FirstFactionOfDef(__result.kindDef.GetModExtension<DefaultTilteFactionExtension>().faction);
                if (faction != null)
                {
                    foreach (RoyalTitle item in __result.royalty.AllTitlesForReading)
                    {
                        item.faction = faction;
                    }
                }
            }
        }
    }
}