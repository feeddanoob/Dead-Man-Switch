using RimWorld;
using UnityEngine;
using Verse;
using static UnityEngine.UI.Image;

namespace DMS
{
    public class FlyByThing : ThingWithComps
    {
        protected ModExt_FlyByThing ext => def.GetModExtension<ModExt_FlyByThing>();

        protected float ageTicks, angle;

        public Vector3 vector = Vector3.forward;

        public bool ShouldDiscard => ageTicks > 0 && !DrawPos.ToIntVec3().InBounds(Map);

        public override Vector3 DrawPos
        {
            get
            {
                return base.DrawPos + def.skyfaller.speed * vector.normalized * ageTicks / 60;
            }
        }

        protected Graphic shadowGraphic;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            vector = vector.Yto0();
            angle = vector.AngleFlat();
            shadowGraphic = ext?.shadowGraphic?.Graphic;
            if (!respawningAfterLoad) ageTicks = -vector.magnitude * 60 / def.skyfaller.speed;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            var tempLoc = drawLoc;
            tempLoc.z += def.skyfaller.zPositionCurve?.Evaluate(ageTicks) ?? 0;
            Graphic.Draw(tempLoc, default, this, angle);
            tempLoc = drawLoc;
            tempLoc.y = Altitudes.AltitudeFor(AltitudeLayer.FloorCoverings);
            shadowGraphic?.Draw(tempLoc, default, this, angle);
        }

        public override void Tick()
        {
            base.Tick();
            ageTicks++;
            if (ShouldDiscard)
            {
                DeSpawn();
                Destroy();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ageTicks, "ageTicks");
            Scribe_Values.Look(ref vector, "vector");
        }
    }

    public class ModExt_FlyByThing : DefModExtension
    {
        public GraphicData shadowGraphic;
    }
}
