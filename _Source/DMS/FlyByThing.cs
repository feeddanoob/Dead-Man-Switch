using UnityEngine;
using Verse;
using static UnityEngine.UI.Image;

namespace DMS
{
    public class FlyByThing : ThingWithComps
    {
        float ageTicks, angle;

        public Vector3 vector = Vector3.forward;

        public bool ShouldDiscard => ageTicks > 0 && !DrawPos.ToIntVec3().InBounds(Map);

        public override Vector3 DrawPos
        {
            get
            {
                return base.DrawPos + def.skyfaller.speed * vector.normalized * ageTicks;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            vector = vector.Yto0();
            angle = vector.AngleFlat();
            if (!respawningAfterLoad) ageTicks = vector.magnitude / def.skyfaller.speed;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Graphic.Draw(drawLoc, default, this, angle);
        }

        public override void Tick()
        {
            base.Tick();
            ageTicks++;
            if (ShouldDiscard)
            {
                DeSpawn();
                Discard();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ageTicks, "ageTicks");
            Scribe_Values.Look(ref vector, "vector");
        }
    }
}
