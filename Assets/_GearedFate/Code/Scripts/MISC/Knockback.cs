// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// Knockback is a script that will knockback the object it is attached to when called.
    /// </summary>
    public class Knockback : MonoBehaviour
    {
        [HideInInspector]
        public bool isKnockedback;

        public Vector2 KnockBackVelocity { get; private set; }

        public void GetKnockedBack(Transform damageSource, float knockbackThrust)
        {
            if (TryGetComponent(out Player player) && player.isInvulnerable)
            {
                return;
            }

            isKnockedback = true;
            KnockBackVelocity = knockbackThrust * (transform.position - damageSource.position).normalized;
        }

        public void GetKnockedBack(Vector2 damageSource, float knockbackThrust)
        {
            if (TryGetComponent(out Player player) && player.isInvulnerable)
            {
                return;
            }

            isKnockedback = true;
            KnockBackVelocity = knockbackThrust * ((Vector2)transform.position - damageSource).normalized;
        }
    }
}
