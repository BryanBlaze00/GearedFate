//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// CreditsTrigger
    /// </summary>
    public class CreditsTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player)) GameManager.Instance.LoadCredits();
        }
    }
}
