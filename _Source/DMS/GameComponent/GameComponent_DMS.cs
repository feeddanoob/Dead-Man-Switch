using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace DMS
{
    public class GameComponent_DMS : GameComponent
    {
        public GameComponent_DMS(Game game) { }
        public List<OutgoingMech> OutgoingMeches 
        {
            get 
            {
                if (outgoingMeches == null) 
                {
                    this.outgoingMeches = new List<OutgoingMech>();
                }
                return this.outgoingMeches;
            }
        }
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
            if (this.removedCache != null && this.removedCache.Any()) 
            {
                this.OutgoingMeches.RemoveAll(m => this.removedCache.Contains(m.mech));
                this.removedCache.Clear();
            }
            foreach (var item in OutgoingMeches)
            {
                item.Tick();
            }
            this.timeToTriggerOutgoing++;
            if (this.timeToTriggerOutgoing >= GenDate.TicksPerSeason) 
            {
                this.timeToTriggerOutgoing = 0;
                Map map = Find.AnyPlayerHomeMap;
                if (map != null && map.mapPawns.SpawnedColonyMechs.Find(m => m.TryGetComp<CompDeadManSwitch>() != null)
                    is Pawn mech) 
                {
                    float mood = 0;
                    foreach (var item in map.mapPawns.FreeColonists)
                    {
                        if (item.needs?.mood != null)
                        {
                            mood += item.needs.mood.CurLevelPercentage;
                        }
                    }
                    mood /= map.mapPawns.ColonistCount;
                    if ((mood <= 0.1 || mood >= 0.9) && Rand.Chance(0.25f)) 
                    {
                        Find.LetterStack.ReceiveLetter("DMS_WokenMechLeave".Translate(mech.Name.ToString()), "DMS_WokenMechLeaveDesc".Translate(mech.Name.ToString()),LetterDefOf.PositiveEvent,mech);
                        mech.TryGetComp<CompDeadManSwitch>().outgoing = true;
                    }
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
            Scribe_Values.Look(ref this.timeToTriggerOutgoing, "timeToTriggerOutgoing");
            Scribe_Collections.Look(ref this.lostMechs, "lostMechs", LookMode.Reference);
            Scribe_Collections.Look(ref this.outgoingMeches, "outgoingMechs", LookMode.Deep);
        }

        public int timeToTriggerOutgoing = 0;
        public int timeToReturn = -1;
        public List<Thing> lostMechs = new List<Thing>();

        public List<Pawn> removedCache = new List<Pawn>();
        private List<OutgoingMech> outgoingMeches = new List<OutgoingMech>();
    }

    public class OutgoingMech : IExposable
    {
        public OutgoingMech() 
        {
            this.timeToTrigger = Rand.Range(5 * GenDate.TicksPerDay,30 * GenDate.TicksPerDay);
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref this.tick, "DMS_TickOutGo");
            Scribe_Values.Look(ref this.timeToTrigger, "DMS_TimeToTrigger");
            Scribe_References.Look(ref this.mech, "DMS_OutGomech");
        }
        public void Tick()
        {
            this.tick++;
            if (this.tick >= this.timeToTrigger)
            {
                this.tick = 0;
                Trigger();
            }
        }

        public void Trigger()
        {
            this.timeToTrigger = Rand.Range(5 * GenDate.TicksPerDay, 30 * GenDate.TicksPerDay);
            if (Rand.Chance(0.25f))
            {
                Settlement s = (Settlement)Find.World.worldObjects.AllWorldObjects.FindAll(w => w.def == WorldObjectDefOf.Settlement &&
                w.Faction != null && !w.Faction.IsPlayer && w.Faction.HostileTo(Find.FactionManager.OfPlayer)).RandomElement();
                Find.LetterStack.ReceiveLetter("DMS_MechStory_Attack".Translate(this.mech.Label), GrammarResolver.Resolve("_story", new GrammarRequest()
                {
                    Includes =
                    {
                        DMS_DefOf.DMS_Outgoing_Attack
                    },
                    Rules =
                        {
                            new Rule_String("Mech",this.mech.Label),
                            new Rule_String("MechType",this.mech.kindDef.race.label)
                        }
                }), LetterDefOf.PositiveEvent, new LookTargets(s.Tile));
                s.Destroy();
            }
            else if (Rand.Chance(0.5f))
            {
                Map playerMap = Find.AnyPlayerHomeMap;
                IntVec3 pos = DropCellFinder.TradeDropSpot(playerMap);
                ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(DMS_DefOf.DMS_OutgoingLoots.root.Generate());
                DropPodUtility.MakeDropPodAt(pos, playerMap, activeDropPodInfo);
                Find.LetterStack.ReceiveLetter("DMS_MechStory_Loot".Translate(this.mech.Label), GrammarResolver.Resolve("_story", new GrammarRequest()
                {
                    Includes =
                    {
                        DMS_DefOf.DMS_Outgoing_Loot
                    },
                    Rules =
                        {
                            new Rule_String("Mech",this.mech.Label),
                            new Rule_String("MechType",this.mech.kindDef.race.label)
                        }
                }), LetterDefOf.PositiveEvent, new LookTargets(pos, playerMap));
            }
            Current.Game.GetComponent<GameComponent_DMS>().OutgoingMeches.Remove(this);
        }

        public Pawn mech;
        public int tick;
        public int timeToTrigger;
    }
}
