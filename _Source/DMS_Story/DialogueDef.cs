using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DMS_Story
{
    public class DialogueDef : Def
    {
        public override void PostLoad()
        {
            base.PostLoad();
            this.worker.def = this;
            this.reacts.SortBy(r => r.priority);
        }
        public string title;
        public string subTitle;
        public DialogueWorker worker = new DialogueWorker();
        public List<ReactDef> reacts = new List<ReactDef>();
    }

    public class ReactDef : Def 
    {
        public int priority;
        public List<OptionDef> options = new List<OptionDef>();
        public bool hasReventOption = false;

        public DiaNode GetNode(Pawn negotiant, FactionNegotiant factionNegotiant, DiaNode last)
        {
            DiaNode result = new DiaNode(this.description);
            this.options.ForEach(o => 
            {
                if (o.worker.Visible(negotiant))
                {
                    DiaOption option = new DiaOption(o.optionLabel);
                    if (!o.worker.Available(negotiant,out string reason))
                    {
                        option.Disable(reason);
                    }
                    option.action = () => o.worker.Work(negotiant, factionNegotiant);
                    if (o.nextReact != null)
                    {
                        option.link = o.nextReact.GetNode(negotiant, factionNegotiant, result);
                    }
                    else 
                    {
                        option.resolveTree = true;
                    }
                    result.options.Add(option);
                }
            });
            if (this.hasReventOption)
            {
                result.options.Add(new DiaOption("Back".Translate()) {link = last});
            }

            return result;
        }
    }
    public class OptionDef : Def 
    {
        public override void PostLoad()
        {
            base.PostLoad();
            this.worker.def = this;
        }
        public string optionLabel;
        public OptionWorker worker = new OptionWorker();
        public ReactDef nextReact;
    }
}
