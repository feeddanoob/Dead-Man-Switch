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
    public class HumanlikeMechExtension : DefModExtension
    {
        public BodyTypeDef bodyTypeOverride;
        public Vector3 headOffset;
        public GraphicData headGraphic;
    }
}
