using System.Linq;
using Verse;

namespace DMS_Story
{
    public class DialogueWorker_Random : DialogueWorker
    {
        public override DiaNode GetNode(Pawn negotiant, FactionNegotiant factionNegotiant, DiaNode last)
        {
            ReactDef start = this.def.reacts.Where(d => d.GetModExtension<DialogueOptionExtension>().Available(negotiant, out var failReason)).RandomElement();
            return start.GetNode(negotiant, factionNegotiant, last);
        }
    }
}
