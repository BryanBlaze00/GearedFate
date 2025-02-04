using UnityEngine;

namespace BTG
{
    public interface IBoss
    {
        public float MaxHealth { get; }

        public float CurrentHealth { get; }
    }
}
