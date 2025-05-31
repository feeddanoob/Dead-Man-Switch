using Verse;
using HarmonyLib;
using RimWorld;

namespace DMS
{
    //由於BossgroupDef綁定機械師所以要這樣改。

    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(CallBossgroupUtility), "BossgroupEverCallable")]
    public static class Patch_BossgroupEverCallable
    {
        public static bool Prefix(BossgroupDef def, ref AcceptanceReport __result)
        {
            if (def.HasModExtension<CallableExtension>())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
    public class CallableExtension : DefModExtension { }
}
