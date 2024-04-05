using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using VFE.Mechanoids;
using VFECore;

namespace tete
{

    [StaticConstructorOnStartup]
    static class HarmonyInit
    {

        static HarmonyInit() {
            new Harmony("vefpatch").PatchAll();
        }
    }


    [HarmonyPatch(typeof(CompMachineChargingStation))]
   
    static class CompMachineChargingStation_Patch
    {
        [HarmonyPatch(nameof(CompMachineChargingStation.PostSpawnSetup))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CompMachineChargingStation_PostDeSpawn(IEnumerable<CodeInstruction> instructions)
        {

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, m_ClearCache);
            foreach (var item in instructions)
            {
                yield return item;
            }

        }
        static MethodInfo m_ClearCache = SymbolExtensions.GetMethodInfo((CompMachineChargingStation comp) => CompMachineChargingStation_Patch.ClearCache(comp));
        public static void ClearCache(CompMachineChargingStation comp)
        {
            if (comp == null || comp.parent == null) return;
            //Log.Message($"cachedChargingStationsDict: {CompMachineChargingStation.cachedChargingStationsDict.Count}; cachedChargingStations: {CompMachineChargingStation.cachedChargingStations.Count}");
            foreach (var (key,val) in CompMachineChargingStation.cachedChargingStationsDict)
            {
                if (val.parent.thingIDNumber == comp.parent.thingIDNumber)
                {
                    CompMachineChargingStation.cachedChargingStations.Remove(val);
                    CompMachineChargingStation.cachedChargingStationsDict.Remove(key);
                    return;
                }
            }
            //Log.Error($"Unable to find a comp for thingid {comp.parent.ThingID} in CompMachineChargingStation");

        }
    }

    [HarmonyPatch(typeof(CompMachine))]
    [HarmonyPatch(nameof(CompMachine.OnBuildingDestroyed))]
    static class CompMachine_Patch {

        
/*        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CompMachine_OnBuildingDestroyed(IEnumerable<CodeInstruction> instructions)
        {
            var li = instructions.ToList();
            var ret = new Label();
            var ctn = new Label();
            li.Last().labels.Add(ret);
            li[5].labels.Add(ctn);

            bool found = false;

            foreach (var i in li)
            {   
                yield return i;
                if (!found && i.opcode == OpCodes.Nop)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Callvirt,AccessTools.Method(typeof(CompPawnDependsOn), "get_MyPawnIsAlive"));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ceq);
                    yield return new CodeInstruction(OpCodes.Brfalse_S, ctn);
                    yield return new CodeInstruction(OpCodes.Br_S, ret);
                }
                
            }
        }*/

        [HarmonyPrefix]
        static bool NoDeadKill(CompPawnDependsOn compPawnDependsOn)
        {
            return compPawnDependsOn.MyPawnIsAlive;
        }

    }


}
