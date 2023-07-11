using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;
using UnityEngine;

using HarmonyLib;

namespace DMS
{
    //public static class Patch_FloatMenuMakerMap
    //{
    //    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    //    public static class AddHumanlikeOrders_Fix
    //    {
    //        public static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
    //        {
    //            IntVec3 c = IntVec3.FromVector3(clickPos);
    //            if (pawn.equipment != null)
    //            {
    //                List<Thing> thingList = c.GetThingList(pawn.Map);
    //                for (int i = 0; i < thingList.Count; i++)
    //                {
    //                    var comp = thingList[i].TryGetComp<CompMechOnlyWeapon>();
    //                    if (comp != null)
    //                    {
    //                        var equipment = (ThingWithComps)thingList[i];
    //                        TaggedString toCheck = "Equip".Translate(equipment.LabelShort);
    //                        FloatMenuOption floatMenuOption = opts.FirstOrDefault((FloatMenuOption x) => x.Label.Contains(toCheck));
    //                        Log.Message(pawn.def.race.ToString());
    //                        if (floatMenuOption != null && comp.IsAllowedRaces(pawn.def.race.ToString()))
    //                        {
    //                            opts.Remove(floatMenuOption);
    //                            opts.Add(new FloatMenuOption("DMS.CannotEquip.RaceNotSupported".Translate(equipment.LabelShort), null));
    //                        }
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
