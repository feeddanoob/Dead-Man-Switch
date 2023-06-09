using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using UnityEngine.WSA;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace DMS
{
    //中控機體，能夠無視機械師控制範圍活動的同時自身也能作為控制範圍的延伸，但是在遠征時控制範圍會因距離而衰減
    public class CompCommandRelay : ThingComp
    {
        public float currentRadius;
        public CompProperties_CommandRelay Props => (CompProperties_CommandRelay)this.props;
        public override void PostDraw()
        {
            base.PostDraw();
            if (Pawn.Drafted)
            {
                if (SameMap)
                {
                    currentRadius = Props.maxRelayRadius;           
                }
                else
                {
                    int num = Find.WorldGrid.TraversalDistanceBetween(Pawn.Map.Tile, Pawn.GetOverseer().Map.Tile);
                    if (num > Props.maxWorldMapRadius)
                    {
                        currentRadius = Props.minRelayRadius;
                    }
                    else
                    {
                        currentRadius = Mathf.Lerp(Props.minRelayRadius, Props.maxRelayRadius, (float)num / (float)Props.maxWorldMapRadius);
                    }
                }
                GenDraw.DrawRadiusRing(this.parent.Position, currentRadius);
            }
        }
        Pawn Pawn => this.parent as Pawn;
        private bool SameMap => Pawn.Map == Pawn.GetOverseer().Map;
    }
    public class CompProperties_CommandRelay : CompProperties
    {
        public int maxWorldMapRadius;
        public float maxRelayRadius;
        public float minRelayRadius;
        public CompProperties_CommandRelay() { this.compClass = typeof(CompCommandRelay); }
    }
}
