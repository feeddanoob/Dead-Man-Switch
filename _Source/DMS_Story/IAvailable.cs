using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DMS_Story
{
    public interface IAvailable
    {
        bool Available(Pawn negotiant, out string failReason);
        bool Visible(Pawn negotiant);
    }
}
