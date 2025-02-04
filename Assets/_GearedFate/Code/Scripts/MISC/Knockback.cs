//
// Copyright (c) BTG. All rights reserved.
//

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// Knockback is a script that will knockback the object it is attached to when called.
    /// </summary>
    public class Knockback : MonoBehaviour
    {
        [HideInInspector] public bool isKnockedback;
        public Vector2 KnockBackVelocity { get; private set; }

        public void GetKnockedBack(Transform damageSource, float knockbackThrust)
        {
            if (this.TryGetComponent(out Player player) && player.isInvulnerable) return;
            this.isKnockedback = true;
            this.KnockBackVelocity = knockbackThrust * (this.transform.position - damageSource.position).normalized;
        }

        public void GetKnockedBack(Vector2 damageSource, float knockbackThrust)
        {
            if (this.TryGetComponent(out Player player) && player.isInvulnerable) return;
            this.isKnockedback = true;
            this.KnockBackVelocity = knockbackThrust * ((Vector2)this.transform.position - damageSource).normalized;
        }
    }
}
