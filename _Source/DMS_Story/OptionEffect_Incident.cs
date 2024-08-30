using RimWorld;
using Verse;

namespace DMS_Story
{
    public class OptionEffect_Incident : OptionEffect
    {
        public override void Work(Pawn negotiant,FactionNegotiant factionNegotiant) 
        {
            this.incident.Worker.TryExecute(new IncidentParms() {target = negotiant.Map});
        }

        public IncidentDef incident;
    }
}
