using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DMS
{
    public class CompUseEffect_CallBossgroupByItem : CompUseEffect
    {
        private Effecter prepareEffecter;

        private int delayTicks = -1;

        public CompProperties_Useable_CallBossgroupByItem Props => (CompProperties_Useable_CallBossgroupByItem)props;

        public bool ShouldSendSpawnLetter
        {
            get
            {
                if (Props.spawnLetterLabelKey.NullOrEmpty() || Props.spawnLetterTextKey.NullOrEmpty())
                {
                    return false;
                }

                if (!MechanitorUtility.AnyMechanitorInPlayerFaction())
                {
                    return false;
                }

                if (Find.BossgroupManager.lastBossgroupCalled > 0)
                {
                    return false;
                }

                return true;
            }
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            if (Props.effecterDef != null)
            {
                Effecter effecter = new Effecter(Props.effecterDef);
                effecter.Trigger(new TargetInfo(parent.Position, parent.Map), TargetInfo.Invalid);
                effecter.Cleanup();
            }

            prepareEffecter?.Cleanup();
            prepareEffecter = null;
            if (Props.delayTicks <= 0)
            {
                CallBossgroup();
            }
            else
            {
                delayTicks = Props.delayTicks;
            }
        }

        private void CallBossgroup()
        {
            GameComponent_Bossgroup component = Current.Game.GetComponent<GameComponent_Bossgroup>();
            if (component == null)
            {
                Log.Error("Trying to call bossgroup with no GameComponent_Bossgroup.");
            }
            else
            {
                Props.bossgroupDef.Worker.Resolve(parent.Map, component.NumTimesCalledBossgroup(Props.bossgroupDef));
            }
        }

        public override TaggedString ConfirmMessage(Pawn p)
        {
            GameComponent_Bossgroup component = Current.Game.GetComponent<GameComponent_Bossgroup>();
            return "BossgroupWarningDialog".Translate(NamedArgumentUtility.Named(Props.bossgroupDef.boss.kindDef, "LEADERKIND"), Props.bossgroupDef.GetWaveDescription(component.NumTimesCalledBossgroup(Props.bossgroupDef)).Named("PAWNS"));
        }
        public override void PrepareTick()
        {
            if (Props.prepareEffecterDef != null && prepareEffecter == null)
            {
                prepareEffecter = Props.prepareEffecterDef.Spawn(parent.Position, parent.MapHeld);
            }
            prepareEffecter?.EffectTick(parent, TargetInfo.Invalid);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (delayTicks >= 0)
            {
                return new AcceptanceReport("AlreadyUsed".Translate());
            }
            AcceptanceReport acceptanceReport = Props.bossgroupDef.Worker.CanResolve(p);
            return acceptanceReport;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!ModLister.CheckBiotech("Call bossgroup"))
            {
                parent.Destroy();
            }
            else if (!respawningAfterLoad && ShouldSendSpawnLetter)
            {
                Props.SendBossgroupDetailsLetter(Props.spawnLetterLabelKey, Props.spawnLetterTextKey, parent.def);
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref delayTicks, "delayTicks", -1);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (delayTicks >= 0)
            {
                delayTicks--;
            }

            if (delayTicks == 0)
            {
                CallBossgroup();
            }
        }
    }
    public class CompProperties_Useable_CallBossgroupByItem : CompProperties_UseEffect
    {
        public BossgroupDef bossgroupDef;

        public EffecterDef effecterDef;

        public EffecterDef prepareEffecterDef;

        [NoTranslate]
        public string spawnLetterTextKey;

        [NoTranslate]
        public string spawnLetterLabelKey;

        [NoTranslate]
        public string unlockedLetterTextKey;

        [NoTranslate]
        public string unlockedLetterLabelKey;

        public int delayTicks = -1;

        private List<ThingDef> tmpMechsUsingRewards = new List<ThingDef>();

        public CompProperties_Useable_CallBossgroupByItem()
        {
            compClass = typeof(CompUseEffect_CallBossgroupByItem);
        }

        public override void Notify_PostUnlockedByResearch(ThingDef parent)
        {
            if (Find.TickManager.TicksGame > 0 && unlockedLetterLabelKey.NullOrEmpty() && !unlockedLetterTextKey.NullOrEmpty())
            {
                SendBossgroupDetailsLetter(unlockedLetterLabelKey, unlockedLetterTextKey, parent);
            }
        }

        public void SendBossgroupDetailsLetter(string labelKey, string textKey, ThingDef parent)
        {
            List<ThingDef> list = new List<ThingDef> { parent };
            list.AddRange(bossgroupDef.boss.kindDef.race.killedLeavingsPlayerHostile.Select((ThingDefCountClass t) => t.thingDef));
            Find.LetterStack.ReceiveLetter(FormatLetterLabel(labelKey), FormatLetterText(textKey, parent), LetterDefOf.NeutralEvent, null, null, null, list);
        }

        public string FormatLetterLabel(string label)
        {
            return label.Translate(NamedArgumentUtility.Named(bossgroupDef.boss.kindDef, "LEADER"));
        }

        public string FormatLetterText(string text, ThingDef parent)
        {
            string arg = bossgroupDef.boss.kindDef.race.killedLeavingsPlayerHostile.Select((ThingDefCountClass r) => r.Label + " x" + r.count).ToLineList("- ");
            return text.Translate(NamedArgumentUtility.Named(parent, "PARENT"), NamedArgumentUtility.Named(bossgroupDef.boss.kindDef, "LEADER"), arg.Named("REWARDSLIST"));
        }
    }
}
