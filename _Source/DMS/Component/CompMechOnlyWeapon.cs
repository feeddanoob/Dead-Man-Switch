using System.Collections.Generic;
using Verse;

namespace DMS
{
    //重型武器的物品形式，由Pawn右鍵後判斷是否能夠裝備，然後
    public class HeavyThing : Thing
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            return base.GetFloatMenuOptions(selPawn);
        }
    }
    //用於被裝備的重型武器掉落時變為特定Thing。
    public class CompHeavyOnDrop : ThingComp
    {
        public override void CompTick()
        {
            base.CompTick();
        }
    }
    public class HeavyEquippableExtension : DefModExtension
    {
        public ThingDef weaponDef;
        public ThingDef equippableRaceDef;
    }
}
