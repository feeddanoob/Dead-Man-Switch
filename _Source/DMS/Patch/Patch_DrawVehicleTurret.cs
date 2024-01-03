using Verse;
using UnityEngine;

using HarmonyLib;

namespace DMS
{
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
	public class Patch_DrawVehicleTurret
	{
		[HarmonyPriority(600)]
		public static bool Prefix(PawnRenderer __instance, Vector3 rootLoc, Rot4 pawnRotation, PawnRenderFlags flags)
		{
			CompVehicleWeapon compWeapon = CompVehicleWeapon.cachedVehicles.TryGetValue(__instance);

			if (compWeapon != null)
			{
				Pawn vehicle = (Pawn)compWeapon.parent;
				if (vehicle.equipment != null && vehicle.equipment.Primary != null)
				{
					DrawTuret(vehicle, compWeapon, vehicle.equipment.Primary);
				}

				return false;
			}
			return true;
		}

		public static void DrawTuret(Pawn pawn, CompVehicleWeapon compWeapon, Thing equipment)
		{
			float aimAngle = compWeapon.CurrentAngle;

			Vector3 drawLoc = pawn.DrawPos;
			drawLoc.y += Altitudes.AltInc;
			drawLoc += compWeapon.Props.drawOffset.RotatedBy(pawn.Rotation.AsAngle);
			float num = (aimAngle - 90f) % 360f;
			Matrix4x4 matrix = default;
			matrix.SetTRS(drawLoc, Quaternion.AngleAxis(num, Vector3.up), new Vector3(equipment.Graphic.drawSize.x, 1f, equipment.Graphic.drawSize.y));
			Graphics.DrawMesh(MeshPool.plane10, matrix, (!(equipment.Graphic is Graphic_StackCount graphic_StackCount)) ? equipment.Graphic.MatSingle : graphic_StackCount.SubGraphicForStackCount(1, equipment.def).MatSingle, 0);
		}
		public static Mesh plane10Flip = MeshMakerPlanes.NewPlaneMesh(1f, true);
	}

}
