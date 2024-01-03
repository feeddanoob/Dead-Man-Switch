using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;

using HarmonyLib;


namespace DMS
{
    // [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    // internal static class Patch_ResolveApparelGraphics
    // {
    //     [HarmonyPrefix]
    //     public static bool DirtyCache(PawnGraphicSet __instance)
    //     {
    //         if(__instance.pawn is HumanlikeMech mech && mech.Extension?.bodyTypeOverride != null)
    //         {
    //             __instance.ResolveApparelGraphicsOverride(mech);
    //             return false;
    //         }
    //         return true;
    //     }

    //     //override: PawnGraphicSet.ResolveApparelGraphics()
    //     public static void ResolveApparelGraphicsOverride(this PawnGraphicSet graphicSet, HumanlikeMech mech)
    //     {
    //         graphicSet.ClearCache();
    //         graphicSet.apparelGraphics.Clear();
    //         using (List<Apparel>.Enumerator enumerator = graphicSet.pawn.apparel.WornApparel.GetEnumerator())
    //         {
    //             while (enumerator.MoveNext())
    //             {
    //                 ApparelGraphicRecord item;
    //                 if (ApparelGraphicRecordGetter.TryGetGraphicApparel(enumerator.Current, mech.Extension.bodyTypeOverride, out item))
    //                 {
    //                     graphicSet.apparelGraphics.Add(item);
    //                 }
    //             }
    //         }
    //     }
    // }
}
