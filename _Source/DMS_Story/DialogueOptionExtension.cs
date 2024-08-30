using DMS_Story;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DMS_Story
{
    public abstract class DialogueOptionExtension : DefModExtension, IAvailable
    {
        public abstract bool Available(Pawn negotiant, out string failReason);

        public abstract bool Visible(Pawn negotiant);
    }
}
