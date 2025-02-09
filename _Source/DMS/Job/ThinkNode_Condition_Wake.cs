﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DMS
{
    public class ThinkNode_Condition_Wake : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.TryGetComp<CompDeadManSwitch>() is CompDeadManSwitch comp && comp.woken;
        }
    }
}
