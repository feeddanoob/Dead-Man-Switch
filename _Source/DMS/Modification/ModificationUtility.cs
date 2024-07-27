using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DMS
{
    public static class ModificationUtility
    {
        public static bool SupportedByRace(Pawn pawn, CompProperties_AddHediffOnTarget comp)//給那種需要涉及渲染的機體改造使用
        {
            if (comp.supportRaceDefs.NullOrEmpty()) return true;//沒寫那就是都可以用。
            return comp.supportRaceDefs.Contains(pawn.def);
        }
        public static BodyPartRecord GetBodyPartRecord(Pawn pawn, CompProperties_AddHediffOnTarget props)
        {
            if (HasSpaceToAttach(pawn, props, out BodyPartRecord b))
            {
                if (b == null) Log.Error("BodyPartNotExists");
                return b;
            }
            return null;
        }
        public static bool HasSpaceToAttach(Pawn pawn, CompProperties_AddHediffOnTarget comp, out BodyPartRecord bodyPart)
        {
            bodyPart = null;
            if (pawn == null)
            {
                Log.Error("pawn is null");
                return false;
            } 
            if (comp == null)
            {
                Log.Error("comp is null");
                return false;
            }
            if (comp.targetBodyPartDefs.NullOrEmpty()) return true;//如果是空的那就是裝全身的

            List<BodyPartRecord> bodyParts = pawn.RaceProps.body.AllParts.Where(p => comp.targetBodyPartDefs.Contains(p.def)).ToList();

            foreach (var hediff in pawn.health.hediffSet.hediffs.Where(h => comp.targetBodyPartDefs.Contains(h.Part.def)))
            {//透過hediff默認安裝位置來找對應hediff,然後從所有零件位置中去除有安裝的部位,不確定有沒有更有效率的方式。
                bodyParts.Remove(hediff.Part);
            }
            if (bodyParts.NullOrEmpty()) return false;
            bodyPart = bodyParts.FirstOrDefault();
            //如果存在沒有安裝仿生體的部位，那就返回true,以及默認第一個。
            return true;
        }
        public static bool HasAnyBodyPartOf(Pawn pawn, List<BodyPartDef> partDefs)
        {
            return !pawn.RaceProps.body.AllParts.Where(p => partDefs.Contains(p.def)).EnumerableNullOrEmpty();
        }
        public static bool HasBodyPartOf(Pawn pawn, BodyPartDef partDef)
        {
            return !pawn.RaceProps.body.AllParts.Where(p => p.def == partDef).EnumerableNullOrEmpty();
        }
    }
}
