using Verse;

namespace DMS
{
    public class SupportDef : Def
    {
        public IntRange triggerTick = new IntRange(120, 150);
        //前處理
        public EffecterDef preEffecterDef;
        public int preTriggerTick = 0;

        //砲擊前訊息，只有在非敵對方觸發時才會顯示。
        public string preMessage = null;

        //觸發
        public EffecterDef triggerEffecter;
        public bool triggerEffecterOnce = false;

        //觸發-彈藥部分
        public ThingDef projectileDef = null;
        public int burstCount = 1, burstInterval = 5;
        public IntRange spreadRadius = new IntRange(5, 10);

        //後處裡
        public EffecterDef postEffecterDef;
        public int postTriggerTick = 0;
    }
}