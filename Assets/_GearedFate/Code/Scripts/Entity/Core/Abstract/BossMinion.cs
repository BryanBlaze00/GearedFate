//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityEngine.AI;

namespace BTG
{
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
        [Header("Boss Minion Stats")] [SerializeField]
        protected float health = 100f;

        [SerializeField] protected float speed = 3.5f;
        [SerializeField] protected float attackCooldown = 2f;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float knockBackAmt = 3f;
        [SerializeField] protected float destroyWaitTime = 2f;
        [SerializeField] protected GameObject explosionEffect;
        [SerializeField] protected Transform target;

        protected Rigidbody2D rb;
        protected NavMeshAgent agent;
        private float attackTimer;

        protected virtual void Awake()
        {
            this.rb = this.GetComponent<Rigidbody2D>();
            this.agent = this.GetComponent<NavMeshAgent>();
            this.NMAgentSetup();
            this.target = FindAnyObjectByType<Player>().transform;
        }

        protected virtual void Start()
        {
            this.attackTimer = this.attackCooldown;
        }

        protected virtual void Update()
        {
            if (this.target != null)
            {
                this.MoveToTarget(this.target);
                this.HandleAttack();
            }
        }

        protected abstract void Attack();

        protected virtual void HandleAttack()
        {
            this.attackTimer -= Time.deltaTime;
            if (this.attackTimer <= 0 && Vector3.Distance(this.transform.position, this.target.position) <= this.attackRange)
            {
                this.Attack();
                this.attackTimer = this.attackCooldown;
            }
        }

        protected virtual void MoveToTarget(Transform target)
        {
            if (target != null && this.agent.enabled)
            {
                this.agent.SetDestination(target.position);
            }
        }

        /// No need to make it virtual as the task is always gonna be same
        public void TakeDamage(float amount)
        {
            Debug.Log("Ouch! from " + this.name + " for " + amount + " damage.");
            this.health -= amount;
            if (this.health <= 0)
            {
                this.Die();
            }
        }

        protected void OnEnable()
        {
            this.agent.enabled = true;
        }

        protected virtual void Die()
        {
            this.agent.enabled = false;
            Instantiate(this.explosionEffect, this.transform.position, Quaternion.identity);
            this.gameObject.SetInactive(this.destroyWaitTime);
        }

        protected virtual void NMAgentSetup()
        {
            this.agent.speed = this.speed;
            this.agent.updateRotation = false;
            this.agent.updateUpAxis = false;
        }
    }
}
