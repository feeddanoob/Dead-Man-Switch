using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace DMS_Story
{
    public class GameComponent_DialogueFramework : GameComponent
    {
        public GameComponent_DialogueFramework(Game game)
        { }
        public FactionNegotiant GetNegotiant(Faction f) 
        {
            if (!this.negotiants.ContainsKey(f) && f.def.GetModExtension<ModExtenson_FactionNegotiant>() is ModExtenson_FactionNegotiant extension) 
            {
                FactionNegotiant n = new FactionNegotiant();
                n.faction = f;
                n.name = GrammarResolver.Resolve("NegotiantName",new GrammarRequest() {Includes = { extension.nameRule } });
                n.goods = extension.goods.RandomElement().root.Generate(new ThingSetMakerParams() {countRange = new IntRange(25,50)});
                n.lastFreshingTick = Find.TickManager.TicksGame;
                this.negotiants.Add(f,n);
            }
            return this.negotiants[f];
        }
        public void SetValue(string name,int init,int value) 
        {
            if (this.globalValue.ContainsKey(name))
            {
                this.globalValue[name] += value;
            }
            else 
            {
                this.globalValue.SetOrAdd(name,init);
            }
        }
        public bool TryGetValue(string name, out int value)
        {
            value = 0;
            if (this.globalValue.ContainsKey(name))
            {
                value = this.globalValue[name];
                return true;
            }
            return false;
        }
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            this.negotiants.ToList().ForEach(n => 
            {
                if (Find.TickManager.TicksGame - n.Value.lastFreshingTick > n.Value.Extension.tickToRefreshGoods) 
                {
                    n.Value.goods = n.Key.def.GetModExtension<ModExtenson_FactionNegotiant>().goods.RandomElement().root.Generate(new ThingSetMakerParams() { countRange = new IntRange(25, 50) });
                }
            });
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.negotiants, "negotiants",LookMode.Reference,LookMode.Deep,ref this.negotiants_f,ref this.negotiants_n);
            Scribe_Collections.Look(ref this.globalValue, "globalValue", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref this.globalBool, "globalValue", LookMode.Value, LookMode.Value);
        }

        public Dictionary<string, int> globalValue = new Dictionary<string, int>();
        public Dictionary<string, bool> globalBool = new Dictionary<string, bool>();
        private Dictionary<Faction, FactionNegotiant> negotiants = new Dictionary<Faction, FactionNegotiant>();
        private List<Faction> negotiants_f = new List<Faction>();
        private List<FactionNegotiant> negotiants_n = new List<FactionNegotiant>();
    }
}
