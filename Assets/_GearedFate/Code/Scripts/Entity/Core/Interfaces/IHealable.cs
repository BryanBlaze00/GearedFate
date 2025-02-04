using UnityEngine;

namespace BTG
{
    public interface IHealable : IAffectable
    {
        public void Heal(float healthAdded);
    }
}
