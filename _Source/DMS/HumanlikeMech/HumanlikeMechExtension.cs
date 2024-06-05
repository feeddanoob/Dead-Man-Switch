using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;

namespace DMS
{
    public class HumanlikeMechExtension : DefModExtension
    {

        public BodyTypeDef bodyTypeOverride;
        public Vector3 headOffset;
        public GraphicData headGraphic;
        public PawnRenderNode headRenderNode;
    }
}
