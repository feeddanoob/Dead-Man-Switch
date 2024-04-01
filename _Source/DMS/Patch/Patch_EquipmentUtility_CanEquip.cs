using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace DMS
{
    [HarmonyPatch(typeof(EquipmentUtility), nameof(EquipmentUtility.CanEquip), new Type[] { typeof(Thing), typeof(Pawn)})]
    internal static class Patch_EquipmentUtility_CanEquip
    {
        [HarmonyPrefix]
        static bool PostFix(ref bool __result, object __0)
        {
            Log.Message("A");
            if (__result)//只有其他判斷過了之後才會接著
            {
                if (Find.Selector.SingleSelectedThing is Pawn selectedPawn)
                {
                    if (selectedPawn is IWeaponUsable && selectedPawn.Faction.IsPlayer)
                    {
                        return true;
                    }
                }

                if (__0 is Thing thing && thing.TryGetComp<CompEquippable>() != null)
                {
                    var extension = thing.def.GetModExtension<HeavyEquippableExtension>();
                    if (extension != null)
                    { 
                        //do something
                    }
                }
            }
            return true;
        }
    }
}
