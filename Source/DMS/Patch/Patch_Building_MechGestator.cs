using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DMS
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(GenRecipe), "PostProcessProduct")]
    static class Patch_PostProcessProduct
    {
        [HarmonyPrefix]
        static bool PreFix(Thing product)
        {
            if (product is WeaponUsableMech pawn)
            {
                pawn.inventory.DestroyAll();
                pawn.equipment.DestroyAllEquipment();
            }
            return true;
        }
    }

    //[StaticConstructorOnStartup]
    //[HarmonyPatch(typeof(Hediff_BandNode), "RecacheBandNodes")]
    //static class Patch_RecacheBandNodes
    //{
    //    [HarmonyPostfix]
    //    static void PostFix(Hediff_BandNode __instance)
    //    {
            
    //        List<Map> maps = Find.Maps;
    //        for (int i = 0; i < maps.Count; i++)
    //        {
    //            foreach (Building item in maps[i].listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.BandNode))
    //            {
    //                if (item.TryGetComp<CompBandNode>().tunedTo == pawn && item.TryGetComp<CompPowerTrader>().PowerOn)
    //                {
    //                    __instance.cachedTunedBandNodesCount++;
    //                }
    //            }
    //        }
    //    }
    //}
}
