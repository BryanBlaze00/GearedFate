// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// SelfDestroy is a script that will destroy the object it is attached to after a set amount of time.
    /// </summary>
    public class SelfDestroy : MonoBehaviour
    {
        [SerializeField]
        private float destroyWaitTime = 1.0f;

        private void Start()
        {
            Destroy(gameObject, destroyWaitTime);
        }
    }
}
