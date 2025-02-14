﻿using Verse;
using HarmonyLib;
using System;

namespace DMS
{
    [HarmonyPatch(typeof(Thing), "get_FlammableNow")]
    public static class Thing_FlammableNow
    {
        [HarmonyPostfix]
        static void Postfix(ref bool __result, Thing __instance)
        {
            if (__result && __instance is IWeaponUsable)
            {
                __result = false;
            }
        }
    }
}
