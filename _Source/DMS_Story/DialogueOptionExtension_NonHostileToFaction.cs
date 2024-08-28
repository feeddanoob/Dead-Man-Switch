using RimWorld;
using Verse;

namespace DMS_Story
{
    /// <summary>
    /// 與DMS陣營非敵對的判斷
    /// </summary>
    public class DialogueOptionExtension_NonHostileToFaction : DialogueOptionExtension
    {
        public override bool Available(Pawn negotiant, out string failReason)
        {
            if (Find.FactionManager.FirstFactionOfDef(DMS_Story.StoryDefOf.DMS_Army).HostileTo(Faction.OfPlayer))
            {
                failReason = "DMSS_PlayerIsNotFriendlyToFaction";
                return false;
            }
            failReason = "";
            return true;
        }
        public override bool Visible(Pawn negotiant)
        {
            return true;
        }
    }
}
