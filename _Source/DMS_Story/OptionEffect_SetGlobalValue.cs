using Verse;

namespace DMS_Story
{
    public class OptionEffect_SetGlobalValue : OptionEffect
    {
        public override void Work(Pawn negotiant, FactionNegotiant factionNegotiant)
        {
            DialogueUtility.Component.SetValue(name,initValue,value);
        }

        public string name;
        public int initValue;
        public int value;
    }
}
