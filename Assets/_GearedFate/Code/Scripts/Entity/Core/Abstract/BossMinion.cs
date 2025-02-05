// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// BossMinion is an abstract class that will control the behavior of the boss minion enemy.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(HitFlash))]
    [RequireComponent(typeof(Knockback))]
    public abstract class BossMinion : MonoBehaviour, IDamagable
    {
        [Header("Boss Minion Stats")]
        [SerializeField]
        protected float health = 100f;

        [SerializeField]
        protected float speed = 3.5f;
        [SerializeField]
        protected float attackCooldown = 2f;
        [SerializeField]
        protected float attackRange = 2f;
        [SerializeField]
        protected float knockBackAmt = 3f;
        [SerializeField]
        protected float destroyWaitTime = 2f;
        [SerializeField]
        protected GameObject explosionEffect;
        [SerializeField]
        protected Transform target;

        protected Rigidbody2D rb;
        protected NavMeshAgent agent;
        private float attackTimer;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            agent = GetComponent<NavMeshAgent>();
            NMAgentSetup();
            target = FindAnyObjectByType<Player>().transform;
        }

        protected virtual void Start()
        {
            attackTimer = attackCooldown;
        }

        protected virtual void Update()
        {
            if (target != null)
            {
                MoveToTarget(target);
                HandleAttack();
            }
        }

        protected abstract void Attack();

        protected virtual void HandleAttack()
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0 && Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }

        protected virtual void MoveToTarget(Transform target)
        {
            if (target != null && agent.enabled)
            {
                agent.SetDestination(target.position);
            }
        }

        /// No need to make it virtual as the task is always gonna be same
        public void TakeDamage(float amount)
        {
            Debug.Log("Ouch! from " + name + " for " + amount + " damage.");
            health -= amount;
            if (health <= 0)
            {
                Die();
            }
        }

        protected void OnEnable()
        {
            agent.enabled = true;
        }

        protected virtual void Die()
        {
            agent.enabled = false;
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            gameObject.SetInactive(destroyWaitTime);
        }

        protected virtual void NMAgentSetup()
        {
            agent.speed = speed;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }
}
