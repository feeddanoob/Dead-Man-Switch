using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;

namespace DMS
{
    public class CompProperties_MechApparel : CompProperties
    {
        [NoTranslate]
        public string gizmoIconPath;

        public CompProperties_MechApparel()
        {
            this.compClass = typeof(CompMechApparel);
        }
    }
}
