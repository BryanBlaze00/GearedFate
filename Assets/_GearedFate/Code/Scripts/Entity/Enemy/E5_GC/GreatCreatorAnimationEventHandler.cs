using System;
using UnityEngine;

namespace BTG
{
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
