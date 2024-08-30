using Verse;

namespace DMS_Story
{
    /// <summary>
    /// 對話者沒穿衣服
    /// </summary>
    public class DialogueOptionExtension_Nudity : DialogueOptionExtension
    {
        public override bool Available(Pawn negotiant, out string failReason)
        {
            failReason = "";
            return !negotiant.apparel.AnyClothing;
        }
        public override bool Visible(Pawn negotiant)
        {
            return Available(negotiant, out var x);
        }
    }
}
