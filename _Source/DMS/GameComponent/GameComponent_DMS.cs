using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DMS
{
    public class GameComponent_DMS : GameComponent
    {
        public GameComponent_DMS(Game game) { }
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (this.lostMechs.Any()) 
            {
                if (this.timeToReturn == -1) 
                {
                    this.timeToReturn = Rand.Range(3 * GenDate.TicksPerDay,10 * GenDate.TicksPerDay);
                }
                this.timeToReturn--;
                if (this.timeToReturn <= 0)
                {
                    ReturnMech();
                }
            }
        }

        public void ReturnMech()
        {
            Pawn mech = (Pawn)this.lostMechs.First();
            CompDeadManSwitch comp = mech.GetComp<CompDeadManSwitch>();
            Pawn overseer = comp.Overseer;
            if (!mech.Spawned && overseer != null && overseer.Map != null && RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 pos,
                overseer.Map, 0.3f))
            {
                GenSpawn.Spawn(mech, pos, overseer.Map);
            }
            this.timeToReturn = Rand.Range(3 * GenDate.TicksPerDay, 10 * GenDate.TicksPerDay);
            Find.LetterStack.ReceiveLetter("DMS_MechReturn".Translate(), "DMS_MechReturnDesc".Translate()
                ,LetterDefOf.PositiveEvent,mech);
            if (Rand.Chance(comp.Props.wakingChance)) 
            {
                comp.woken_Lurk = true;
                comp.timeToWake = Rand.Range(1 * GenDate.TicksPerDay,2 * GenDate.TicksPerDay);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.timeToReturn, "timeToReturn");
            Scribe_Collections.Look(ref this.lostMechs, "lostMechs", LookMode.Reference);
        }

        public int timeToReturn = -1;
        public List<Thing> lostMechs = new List<Thing>();
    }
}
