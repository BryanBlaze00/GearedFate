using UnityEngine;

namespace BTG
{
    public interface IRefuelable : IAffectable
    {
        public void Refuel(float fuelAmount);
    }
}
