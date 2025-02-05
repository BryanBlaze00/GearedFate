// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using Random = UnityEngine.Random;

    /// <summary>
    /// GearboundSentinel
    /// </summary>
    public class GreatCreator : MonoBehaviour, IDamagable, IBoss
    {
        private readonly Dictionary<GreatCreatorState, GCBaseState> _states = new ();

        private readonly FiniteStateMachine<GreatCreatorState> _fsm = new ();

        [SerializeField]
        private Transform _spawnPos;

        private Transform _player;

        public event Action OnHitTaken;

        public enum GreatCreatorState
        {
            Idle,
            Swarm,
            RunAway,
            Dash,
            Transform,
            Spin,
            Death,
        }

        [field: SerializeField]
        public float MoveSpeed { get; private set; }

        [field: SerializeField]
        public float MaxHealth { get; private set; }

        [field: SerializeField]
        public float SafeDistance { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        public List<float> StageTransitionHealthPercentage { get; private set; }

        [field: SerializeField]
        [field: Header("Stage 1 Data")]
        public float ShockDamage { get; private set; }

        [field: SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }

        [field: SerializeField]
        public GreatCreatorAnimationEventHandler AnimationEventHandler { get; private set; }

        [field: SerializeField]
        public MinMaxInt SwarmAmount { get; private set; }

        [field: SerializeField]
        public MinMaxFloat SwarmCoolDown { get; private set; }

        [field: SerializeField]
        public float DashForce { get; private set; }

        [field: SerializeField]
        public Animator Animator { get; private set; }

        public int Stage { get; private set; }

        public float CurrentHealth { get; private set; }

        public int AnimMoveY { get; private set; }

        public int AnimMoveX { get; private set; }

        public float DistanceToTarget => Vector2.Distance(_player.position, transform.position);

        public Transform Target => _player;

        public Vector2 DirectionToTarget => (transform.position - _player.position).normalized;

        [field: SerializeField]
        public NavMeshAgent Agent { get; private set; }

        public GCBaseState this[GreatCreatorState key] => _states[key];

        public void SetAnimationMoveParameters(Vector2 move)
        {
            Animator.SetFloat(AnimMoveX, move.x);
            Animator.SetFloat(AnimMoveY, move.y);
        }

        public void SpawnMinions(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var obj = ObjectPool.Instance.GetPooledObject(Random.Range(0f, 1f) > 0.5f
                    ? PooledObjectType.BombMinion
                    : PooledObjectType.DrillMinion);
                obj.transform.position = (Vector2)_spawnPos.position + (Random.insideUnitCircle * 0.1f);
                obj.SetActive(true);
            }
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
            GetComponent<HitFlash>().HitFlashRoutine();

            if (Stage < StageTransitionHealthPercentage.Count && CurrentHealth < MaxHealth * StageTransitionHealthPercentage[Stage])
            {
                Stage++;
                if (Stage == 1)
                {
                    _fsm.SwitchState(_states[GreatCreatorState.Transform]);
                    ((GCRunAwayState)_states[GreatCreatorState.RunAway]).SetAnimation(
                        Animator.StringToHash("WalkPhase2FourArms"));
                    ((GCSpinState)_states[GreatCreatorState.Swarm]).SetAnimation(Animator.StringToHash("Spin"));
                }

                if (Stage == 2)
                {
                    ((GCRunAwayState)_states[GreatCreatorState.RunAway]).SetAnimation(
                        Animator.StringToHash("WalkPhase2ThreeArms"));
                    _fsm.SwitchState(_states[GreatCreatorState.RunAway]);
                }

                if (Stage == 3)
                {
                    ((GCRunAwayState)_states[GreatCreatorState.RunAway]).SetAnimation(
                        Animator.StringToHash("WalkPhase2TwoArms"));
                    _fsm.SwitchState(_states[GreatCreatorState.RunAway]);
                }
            }

            if (CurrentHealth == 0)
            {
                Elevator.Instance.ActivateElevator();
            }

            OnHitTaken?.Invoke();
        }

        public IEnumerator ShootProjectiles()
        {
            var start = Vector2.up;
            for (var i = 0; i < 12; i++)
            {
                var laser = ObjectPool.Instance.GetPooledObject(PooledObjectType.LaserProjectile);
                laser.GetComponent<Projectile>().SetUnaffectedLayer(LayerMask.NameToLayer("Enemy"));
                laser.transform.position = transform.position;

                laser.SetActive(true);
                laser.GetComponent<Rigidbody2D>().linearVelocity = start * 5;
                start = Quaternion.AngleAxis(30, Vector3.forward) * start;
            }

            yield return new WaitForSeconds(0.5f);

            start = (Vector2.up + Vector2.right).normalized;

            for (var i = 0; i < 12; i++)
            {
                var laser = ObjectPool.Instance.GetPooledObject(PooledObjectType.LaserProjectile);
                laser.GetComponent<Projectile>().SetUnaffectedLayer(LayerMask.NameToLayer("Enemy"));

                laser.SetActive(true);
                laser.transform.position = transform.position;
                laser.GetComponent<Rigidbody2D>().linearVelocity = start * 5;
                start = Quaternion.AngleAxis(30, Vector3.forward) * start;
            }
        }

        private void Start()
        {
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;
            _player = FindFirstObjectByType<Player>().transform;

            _states.Add(
                GreatCreatorState.Idle,
                new GCIdleState(_fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Idle))));
            _states.Add(
                GreatCreatorState.Swarm,
                new GCSwarmState(_fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Swarm))));
            _states.Add(GreatCreatorState.RunAway, new GCRunAwayState(_fsm, this, Animator.StringToHash("Walk")));
            _states.Add(
                GreatCreatorState.Dash,
                new GCDashState(_fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Dash))));
            _states.Add(
                GreatCreatorState.Transform,
                new GCTransformationState(_fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Transform))));
            _states.Add(
                GreatCreatorState.Spin,
                new GCSpinState(_fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Spin))));
            _states.Add(
                GreatCreatorState.Death,
                new GCDeathState(_fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Death))));

            _fsm.Initialize(_states[GreatCreatorState.Idle]);
            Stage = 0;
            CurrentHealth = MaxHealth;

            AnimMoveX = Animator.StringToHash("MoveX");
            AnimMoveY = Animator.StringToHash("MoveY");
        }

        private void Update()
        {
            _fsm.CurrentState.OnFrameUpdate();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<Player>().Knockback.GetKnockedBack(transform, 1f);
                if (_fsm.CurrentState.GetType() == typeof(GCDashState))
                {
                    collision.collider.GetComponent<Player>().TakeDamage(10f);
                }
            }
        }
    }
}
