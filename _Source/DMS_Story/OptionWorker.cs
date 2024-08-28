using System.Collections.Generic;
using Verse;

namespace DMS_Story
{
    public class OptionWorker : IAvailable
    {
        public virtual ReactDef GetNextReact(Pawn negotiant) 
        {
            return this.def.nextReact;
        }
        public virtual bool Available(Pawn negotiant, out string failReason)
        {
            string reason = null;
            bool result = this.def.modExtensions == null || !this.def.modExtensions.FindAll(e => e is DialogueOptionExtension).Exists(e => e is DialogueOptionExtension e2 && e2.Available(negotiant, out reason));
            failReason = reason;
            return result;
        }

        public virtual bool Visible(Pawn negotiant)
        {
            return this.def.modExtensions == null || !this.def.modExtensions.FindAll(e => e is DialogueOptionExtension).Exists(e => e is DialogueOptionExtension e2 && e2.Visible(negotiant));
        }
        
        public virtual void Work(Pawn negotiant, FactionNegotiant factionNegotiant) 
        {
            this.effects.ForEach(e => e.Work(negotiant, factionNegotiant));
        }

        public OptionDef def;   
        public List<OptionEffect> effects = new List<OptionEffect>();
    }
}
