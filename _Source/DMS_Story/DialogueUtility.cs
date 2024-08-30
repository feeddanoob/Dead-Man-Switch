using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DMS_Story
{
    public static class DialogueUtility
    {
        public static GameComponent_DialogueFramework Component 
        {
            get 
            {
                if (Current.Game.GetComponent<GameComponent_DialogueFramework>() is GameComponent_DialogueFramework c)
                {
                    return c;
                }
                else 
                {
                    GameComponent_DialogueFramework c2 = new GameComponent_DialogueFramework(Current.Game);
                    Current.Game.components.Add(c2);
                    return c2;
                }
            }
        }

		[DebugAction("DialogueFramework", null, false, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
		private static void OpenDialogue()
		{
			List<DebugMenuOption> options = new List<DebugMenuOption>();
			DefDatabase<DialogueDef>.AllDefsListForReading.ForEach(d =>
			{
				options.Add(new DebugMenuOption(d.defName, DebugMenuOptionMode.Action, () =>
				{       
                    List<DebugMenuOption> options2 = new List<DebugMenuOption>();
                    Find.FactionManager.AllFactionsListForReading.ForEach(f => 
                    {
                        options2.Add(new DebugMenuOption(f.Name, DebugMenuOptionMode.Tool, () =>
                        {
                            if (UI.MouseCell().GetFirstPawn(Find.CurrentMap) is Pawn p) 
                            {
                                CreateDialogue(p,f,d);
                            }
                        }));
                    });
					Find.WindowStack.Add(new Dialog_DebugOptionListLister(options2));
				}));
			});
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
		}

		public static void CreateDialogue(Pawn negotiant,Faction f,DialogueDef def) 
        {
            Find.WindowStack.Add(new Dialog_Negotiation(negotiant,Component.GetNegotiant(f),def.worker.GetNode(negotiant,Component.GetNegotiant(f),null),false));
        }
    }
}
