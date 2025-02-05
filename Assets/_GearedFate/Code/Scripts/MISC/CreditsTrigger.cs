// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// CreditsTrigger
    /// </summary>
    public class CreditsTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                GameManager.Instance.LoadCredits();
            }
        }
    }
}
