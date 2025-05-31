using Verse;

namespace DMS
{
    public interface IWeaponUsable
    {
        void Equip(ThingWithComps equipment);
        void Wear(ThingWithComps equipment);
    }
}
