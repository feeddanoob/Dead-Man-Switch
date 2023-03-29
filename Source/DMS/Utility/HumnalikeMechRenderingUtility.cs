using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;

using HarmonyLib;

namespace DMS
{
    public static class HumnalikeMechRenderingUtility
    {
		public const float headAltOffeset = 0.0289575271f;

		public static void DrawHeadOverride(this HumanlikeMech mech)
		{
			if (mech.HeadGraphic == null) return;
			Quaternion rotation = Quaternion.AngleAxis(mech.Rotation.AsAngle, Vector3.up);
			Vector3 drawLoc = mech.DrawPos + mech.Extension.headOffset + rotation * mech.BaseHeadOffset() ;
			Matrix4x4 matrix = default;
			drawLoc.y += headAltOffeset;
			matrix.SetTRS(drawLoc, Quaternion.identity, new Vector3(mech.Extension.headGraphic.drawSize.x, 1, mech.Extension.headGraphic.drawSize.x));

			Mesh mesh;

			if ((mech.Rotation == Rot4.West && mech.HeadGraphic.WestFlipped) || (mech.Rotation == Rot4.East && mech.HeadGraphic.EastFlipped))
			{
				mesh = MeshPool.plane10Flip;
			}
			else
            {
				mesh = MeshPool.plane10;
			}


			Graphics.DrawMesh(mesh, matrix, mech.HeadGraphic.MatAt(mech.Rotation), 0);
		}

		public static Vector3 BaseHeadOffset(this HumanlikeMech mech)
		{
			Vector2 headOffset = mech.Extension.headOffset;
			switch (mech.Rotation.AsInt)
			{
				case 0:
					return new Vector3(0f, 0f, headOffset.y);
				case 1:
					return new Vector3(headOffset.x, 0f, headOffset.y);
				case 2:
					return new Vector3(0f, 0f, headOffset.y);
				case 3:
					return new Vector3(0f - headOffset.x, 0f, headOffset.y);
				default:
					Log.Error("BaseHeadOffsetAt error in " + mech);
					return Vector3.zero;
			}
		}
	}
}
