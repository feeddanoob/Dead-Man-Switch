using RimWorld;
using Verse;

namespace DMS_Story
{
    public class OptionEffect_OpenTradeDialog : OptionEffect
    {
        public override void Work(Pawn negotiant, FactionNegotiant factionNegotiant)
        {
            Find.WindowStack.Add(new Dialog_Trade(negotiant, factionNegotiant));
        }
    }
}
