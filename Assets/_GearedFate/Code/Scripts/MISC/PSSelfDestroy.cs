//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PSSelfDestroy is a script that will destroy the object it is attached to after the particle system has finished playing.
    /// </summary>
    public class PSSelfDestroy : MonoBehaviour
    {
        private ParticleSystem ps;

        private void Awake()
        {
            this.ps = this.GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (this.ps && !this.ps.IsAlive())
            {
                this.DestroySelfAnimEvent();
            }
        }

        private void DestroySelfAnimEvent()
        {
            Destroy(this.gameObject);
        }
    }
}
