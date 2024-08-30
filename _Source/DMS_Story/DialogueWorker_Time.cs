using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace DMS_Story
{
    public class DialogueWorker_Time : DialogueWorker
    {
        public override DiaNode GetNode(Pawn negotiant, FactionNegotiant factionNegotiant, DiaNode last)
        {
            int time = GenLocalDate.HourInteger(negotiant.MapHeld);
            ReactDef start = null;
            if (time < 6)//清晨
            {
                start = this.def.reacts[0];
            }
            else if (time < 10)//早上
            {
                start = this.def.reacts[1];
            }
            else if (time < 14)//中午
            {
                start = this.def.reacts[2];
            }
            else if (time < 20)//下午
            {
                start = this.def.reacts[3];
            }
            else//晚上
            {
                start = this.def.reacts[4];
            }
            return start.GetNode(negotiant, factionNegotiant, last);
        }
    }
}
