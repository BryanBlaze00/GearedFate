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
        private float _attackTimer;

        [Header("Boss Minion Stats")]
        [field:SerializeField]
        protected float Health { get; private set; }

        [field:SerializeField]
        protected float Speed { get; private set; }

        [field:SerializeField]
        protected float AttackCooldown { get; private set; }

        [field:SerializeField]
        protected float AttackRange { get; private set; }

        [field:SerializeField]
        protected float KnockBackAmt { get; private set; }

        [field:SerializeField]
        protected float DestroyWaitTime { get; private set; }

        [field:SerializeField]
        protected GameObject ExplosionEffect { get; private set; }

        [field:SerializeField]
        protected Transform Target { get; private set; }

        [field:SerializeField]
        protected Rigidbody2D Rb { get; private set; }

        [field:SerializeField]
        protected NavMeshAgent Agent { get; private set; }

        // No need to make it virtual as the task is always gonna be same
        public void TakeDamage(float amount)
        {
            Debug.Log("Ouch! from " + name + " for " + amount + " damage.");
            Health -= amount;
            if (Health <= 0)
            {
                Die();
            }
        }

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Agent = GetComponent<NavMeshAgent>();
            NMAgentSetup();
            Target = FindAnyObjectByType<Player>().transform;
        }

        protected virtual void Start()
        {
            _attackTimer = AttackCooldown;
        }

        protected virtual void Update()
        {
            if (Target != null)
            {
                MoveToTarget(Target);
                HandleAttack();
            }
        }

        protected abstract void Attack();

        protected void HandleAttack()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0 && Vector3.Distance(transform.position, Target.position) <= AttackRange)
            {
                Attack();
                _attackTimer = AttackCooldown;
            }
        }

        protected void MoveToTarget(Transform target)
        {
            if (target != null && Agent.enabled)
            {
                Agent.SetDestination(target.position);
            }
        }

        protected void OnEnable()
        {
            Agent.enabled = true;
        }

        protected void Die()
        {
            Agent.enabled = false;
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
            gameObject.SetInactive(DestroyWaitTime);
        }

        protected void NMAgentSetup()
        {
            Agent.speed = Speed;
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;
        }
    }
}
