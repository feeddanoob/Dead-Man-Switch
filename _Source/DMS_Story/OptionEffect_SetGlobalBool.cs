using Verse;

namespace DMS_Story
{
    public class OptionEffect_SetGlobalBool : OptionEffect
    {
        public override void Work(Pawn negotiant, FactionNegotiant factionNegotiant)
        {
            DialogueUtility.Component.globalBool.SetOrAdd(name, value);
        }

        public string name;
        public bool value;
    }
}
