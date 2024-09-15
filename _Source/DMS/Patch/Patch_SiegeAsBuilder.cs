using HarmonyLib;
using RimWorld;
using Verse;

namespace DMS
{
    [HarmonyPatch(typeof(LordToil_Siege), "SetAsBuilder")]
    internal class Patch_SiegeAsBuilder
    {
        public static bool Prefix(LordToil_Siege __instance, Pawn p)
        {

            if (p.def.race.IsMechanoid)
            {
                __instance.SetAsDefender(p);
                return false;
            }

            return true;
        }
    }
}

