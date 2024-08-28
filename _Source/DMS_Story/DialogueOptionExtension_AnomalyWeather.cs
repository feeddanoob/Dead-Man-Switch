using RimWorld;
using Verse;

namespace DMS_Story
{
    /// <summary>
    /// 天氣很糟糕的特殊對話
    /// </summary>
    public class DialogueOptionExtension_AnomalyWeather : DialogueOptionExtension 
    {
        public override bool Available(Pawn negotiant, out string failReason)
        {
            failReason = "";
            return negotiant.MapHeld.weatherManager.curWeather != WeatherDefOf.Clear;
        }
        public override bool Visible(Pawn negotiant)
        {
            return Available(negotiant, out var x);
        }
    }
}
