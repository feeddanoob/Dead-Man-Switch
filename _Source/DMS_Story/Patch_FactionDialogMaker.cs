using DMS_Story;
using DMS;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DMS_Story
{
    [HarmonyPatch(typeof(FactionDialogMaker), nameof(FactionDialogMaker.FactionDialogFor))]
    internal static class Patch_FactionDialogMaker
    {
        public static bool Prefix(ref DiaNode __result, Pawn negotiator, Faction faction)
        {
            if (faction.def == StoryDefOf.DMS_Army)
            {
                return true;
                //Map map = negotiator.Map;
                //Pawn pawn;
                //string text;
                //if (faction.leader != null)
                //{
                //    pawn = faction.leader;
                //    text = faction.leader.Name.ToStringFull.Colorize(ColoredText.NameColor);
                //}
                //else
                //{
                //    Log.Error(string.Concat("Faction ", faction, " has no leader."));
                //    pawn = negotiator;
                //    text = faction.Name;
                //}
                //DiaNode root;
                //if (faction.PlayerRelationKind == FactionRelationKind.Hostile)
                //{
                //    string key = ((!faction.def.permanentEnemy && "FactionGreetingHostileAppreciative".CanTranslate()) ? ((faction.def.dialogFactionGreetingHostileAppreciative == null || !(faction.def.dialogFactionGreetingHostileAppreciative != "")) ? "FactionGreetingHostileAppreciative" : faction.def.dialogFactionGreetingHostileAppreciative) : ((faction.def.dialogFactionGreetingHostile == null || !(faction.def.dialogFactionGreetingHostile != "")) ? "FactionGreetingHostile" : faction.def.dialogFactionGreetingHostile));
                //    root = new DiaNode(key.Translate(text).AdjustedFor(pawn));
                //}
                //else if (faction.PlayerRelationKind == FactionRelationKind.Neutral)
                //{
                //    string key2 = "FactionGreetingWary";
                //    if (faction.def.dialogFactionGreetingWary != null && faction.def.dialogFactionGreetingWary != "")
                //    {
                //        key2 = faction.def.dialogFactionGreetingWary;
                //    }
                //    root = new DiaNode(key2.Translate(text, negotiator.LabelShort, negotiator.Named("NEGOTIATOR"), pawn.Named("LEADER")).AdjustedFor(pawn));
                //}
                //else
                //{
                //    string key3 = "FactionGreetingWarm";
                //    if (faction.def.dialogFactionGreetingWarm != null && faction.def.dialogFactionGreetingWarm != "")
                //    {
                //        key3 = faction.def.dialogFactionGreetingWarm;
                //    }
                //    root = new DiaNode(key3.Translate(text, negotiator.LabelShort, negotiator.Named("NEGOTIATOR"), pawn.Named("LEADER")).AdjustedFor(pawn));
                //}
                //if (map != null && map.IsPlayerHome)
                //{
                //}
                //AddAndDecorateOption(new DiaOption("(" + "Disconnect".Translate() + ")")
                //{
                //    resolveTree = true
                //}, needsSocial: false);
                //__result = root;
                //void AddAndDecorateOption(DiaOption opt, bool needsSocial)
                //{
                //    if (needsSocial && negotiator.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
                //    {
                //        opt.Disable("WorkTypeDisablesOption".Translate(SkillDefOf.Social.label));
                //    }
                //    root.options.Add(opt);
                //}
                //return false;
            }
            return true;
        }
    }
    public static class CommsConsole
    {
    }
}
