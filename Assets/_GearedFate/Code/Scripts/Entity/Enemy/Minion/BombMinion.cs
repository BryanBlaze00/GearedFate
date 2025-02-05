namespace BTG
{
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// BombMinion is a class that will control the behavior of the bomb minion enemy.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class BombMinion : BossMinion
    {
        [Header("Bomb Minion Stats")]
        [SerializeField]
        private float explosionDmgAmt = 10.0f;

        [SerializeField]
        private float explodeKnockBackAmt = 5.0f;

        protected override void Attack()
        {
            // BombMinion does not attack, it explodes on contact with the player.
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                if (player.isInvulnerable)
                {
                    return;
                }

                agent.enabled = false;
                player.GetComponent<Knockback>().GetKnockedBack(transform, explodeKnockBackAmt);
                player.TakeDamage(explosionDmgAmt);
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
                gameObject.SetInactive(destroyWaitTime);
            }
        }
    }
}
