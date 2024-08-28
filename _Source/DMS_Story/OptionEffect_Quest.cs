using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace DMS_Story
{
    public class OptionEffect_Quest : OptionEffect
    {
        public override void Work(Pawn negotiant, FactionNegotiant factionNegotiant)
        {
            Slate slate = new Slate();
            slate.Set("Faction",factionNegotiant.faction);
            QuestUtility.GenerateQuestAndMakeAvailable(this.quest, slate);
        }

        public QuestScriptDef quest;
    }
}
