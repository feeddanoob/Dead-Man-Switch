﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DMS
{
    public class PawnRenderNodeWorker_SubTurretGun : PawnRenderNodeWorker
	{
		public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
		{
            if (node is PawnRenderNode_SubTurretGun a)
            {
				return a.subturret.TurretProp.renderNodeProperties[0].drawSize.ToVector3();
            }
            return base.ScaleFor(node, parms);
		}
        public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
		{
			Quaternion quaternion = base.RotationFor(node, parms);
			if (node is PawnRenderNode_SubTurretGun pawnRenderNode_TurretGun)
			{
                quaternion *= pawnRenderNode_TurretGun.subturret.curRotation.ToQuat();
            }
			return quaternion;
		}
	}

}
