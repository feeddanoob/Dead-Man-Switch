using UnityEngine;
using Verse;

namespace DMS
{
    public abstract class AirSupportComp_SetOrigin : AirSupportComp
    {
        bool ignoreOverride;

        public override void Trigger(AirSupportDef def, Thing trigger, Map map, LocalTargetInfo target)
        {
            if (!ignoreOverride && def.originOverridedCache) return;
        }
    }
    public class AirSupportComp_SetOriginRandomEdge : AirSupportComp_SetOrigin
    {
        public override void Trigger(AirSupportDef def, Thing trigger, Map map, LocalTargetInfo target)
        {
            base.Trigger(def, trigger, map, target);

            def.tempOriginCache = CellFinder.RandomEdgeCell(map).ToVector3Shifted();
        }
    }

    public class AirSupportComp_SetOriginAngle : AirSupportComp_SetOrigin
    {
        FloatRange angleRange = new(0, 360);

        public override void Trigger(AirSupportDef def, Thing trigger, Map map, LocalTargetInfo target)
        {
            base.Trigger(def, trigger, map, target);

            def.tempOriginCache = Position(angleRange.RandomInRange, map);
        }

        protected Vector3 Position(float angle, Map map)
        {
            float theta = Mathf.Deg2Rad * angle; // 角度轉換為弧度

            // 計算方向向量
            float dx = Mathf.Sin(theta);
            float dy = -Mathf.Cos(theta);

            float x = map.Center.x;
            float y = map.Center.y;

            while (x >= 0 && x < map.Size.x && y >= 0 && y < map.Size.y)
            {
                x += dx;
                y += dy;
            }

            int edgeX = Mathf.RoundToInt(x - dx);
            int edgeY = Mathf.RoundToInt(y - dy);

            return new Vector3(edgeX, 0, edgeY);
        }
    }
}
