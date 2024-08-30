using System.Linq;
using Verse;

namespace DMS_Story
{
    public class DialogueWorker
    {
        public DialogueDef def;
        public virtual DiaNode GetNode(Pawn negotiant, FactionNegotiant factionNegotiant, DiaNode last) 
        {
            ReactDef start = this.def.reacts.Where(d => d.GetModExtension<DialogueOptionExtension>().Available(negotiant, out var failReason)).First();
            return start.GetNode(negotiant, factionNegotiant, last);
        }

    }
}
