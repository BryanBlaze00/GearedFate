namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Base script for scrap that the player can collect.
    /// </summary>
    public abstract class Scrap : MonoBehaviour
    {
        public abstract void ApplyEffect(IAffectable affectable);
    }
}
