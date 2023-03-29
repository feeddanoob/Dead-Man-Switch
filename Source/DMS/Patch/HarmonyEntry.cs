﻿using System;
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
    [StaticConstructorOnStartup]
    public static class HarmonyEntry
    {
        static HarmonyEntry()
        {
            Harmony entry = new Harmony("AOBA.TheDeadManSwtich");
            entry.PatchAll();
        }

    }
}
