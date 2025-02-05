namespace BTG
{
    using System;
    using UnityEngine;

    public class GreatCreatorAnimationEventHandler : MonoBehaviour
    {
        public event Action OnSpawnFinished;

        [SerializeField]
        private void SpawnFinished()
        {
            OnSpawnFinished?.Invoke();
        }
    }
}
