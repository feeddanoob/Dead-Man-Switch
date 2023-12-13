using HarmonyLib;
using RimWorld;
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
}
