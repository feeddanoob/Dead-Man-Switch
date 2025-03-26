using UnityEngine;
using Verse;

namespace DMS
{
    public class AirSupportData_SpawnFlyBy : AirSupportData_SpawnThing
    {
        public Vector3 origin;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
        }

        protected override void PreProcess(Thing t)
        {
            var flyby = t as FlyByThing;
            flyby.vector = target.Cell.ToVector3Shifted() - origin;
        }
    }
}
