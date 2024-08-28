using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace DMS_Story
{
    public class FactionNegotiant : IExposable, ICommunicable,ITrader
    {
        public ModExtenson_FactionNegotiant Extension 
        {
            get 
            {
                if (this.extension == null) 
                {
                    this.extension = this.faction.def.GetModExtension<ModExtenson_FactionNegotiant>();
                }
                return this.extension;
            }
        }
        public TraderKindDef TraderKind => this.Extension.traderKind;

        public IEnumerable<Thing> Goods => this.goods;

        public int RandomPriceFactorSeed => 1145;

        public string TraderName => this.name;

        public bool CanTradeNow => true;

        public float TradePriceImprovementOffsetForPlayer => 0f;

        public Faction Faction => this.faction;

        public TradeCurrency TradeCurrency => TradeCurrency.Silver;
        public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
        {
            return null;
        }

        public string GetCallLabel()
        {
            return this.name;
        }

        public Faction GetFaction()
        {
            return this.faction;
        }

        public string GetInfoText()
        {
            return this.name;
        }

        public void TryOpenComms(Pawn negotiator)
        {
 
        }

        public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
        {
            IEnumerable<Thing> enumerable = from x in playerNegotiator.Map.listerThings.AllThings
                                            where
                                           x.def == ThingDefOf.Silver && TradeUtility.PlayerSellableNow(x, this) && !x.IsForbidden(playerNegotiator) && !x.Position.Fogged(x.Map)
                                            select x;
            foreach (Thing thing in enumerable)
            {
                yield return thing;
            }
            yield break;
        }
        public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            toGive.DeSpawn();
        }


        public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Pawn pawn = toGive as Pawn;
            if (pawn != null)
            {
                pawn.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
                Lord lord = pawn.GetLord();
                if (lord != null)
                {
                    lord.Notify_PawnLost(pawn, PawnLostCondition.Undefined, null);
                }
            }
            else
            {
                Map map = playerNegotiator.Map;
                IntVec3 positionHeld = DropCellFinder.TradeDropSpot(map);
                Thing thing = toGive.SplitOff(countToGive);
                thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
                if (GenPlace.TryPlaceThing(thing, positionHeld, map, ThingPlaceMode.Near, null, null, default(Rot4)))
                {
                }
                else
                {
                    Log.Error(string.Concat(new object[]
                    {
                        "Could not place bought thing ",
                        thing,
                        " at ",
                        positionHeld
                    }));
                    thing.Destroy(DestroyMode.Vanish);
                }
            }
            if (this.goods.Contains(toGive))
            {
                this.goods.Remove(toGive);
            }
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref this.name, "name");
            Scribe_Values.Look(ref this.lastFreshingTick, "lastFreshingTick");
            Scribe_References.Look(ref this.faction, "faction");
            Scribe_Collections.Look(ref this.goods,"goods",LookMode.Deep);
        }

        public string name;
        public int lastFreshingTick = 0;
        public Faction faction;
        public List<Thing> goods = new List<Thing>();  

        public ModExtenson_FactionNegotiant extension;
    }
}
