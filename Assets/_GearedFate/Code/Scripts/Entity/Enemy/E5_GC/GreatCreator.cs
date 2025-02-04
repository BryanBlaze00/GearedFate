//
// Copyright (c) BTG. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace BTG
{
    /// <summary>
    /// GearboundSentinel
    /// </summary>
    public class GreatCreator : MonoBehaviour, IDamagable, IBoss
    {
        public enum GreatCreatorState
        {
            Idle,
            Swarm,
            RunAway,
            Dash,
            Transform,
            Spin,
            Death
        }

        public Action OnHitTaken;

        public readonly Dictionary<GreatCreatorState, GCBaseState> States = new();

        [field: SerializeField] public float MoveSpeed { get; private set; }

        [field: SerializeField] public float MaxHealth { get; private set; }

        [field: SerializeField] public float SafeDistance { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        public List<float> StageTransitionHealthPercentage { get; private set; }

        [field: SerializeField]
        [field: Header("Stage 1 Data")]
        public float ShockDamage { get; private set; }

        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }

        [field: SerializeField] public GreatCreatorAnimationEventHandler AnimationEventHandler { get; private set; }

        [field: SerializeField] public MinMaxInt SwarmAmount;

        [field: SerializeField] public MinMaxFloat SwarmCoolDown;

        [field: SerializeField] public float DashForce { get; private set; }

        [field: SerializeField] public Animator Animator { get; private set; }

        public int Stage { get; private set; }

        public float CurrentHealth { get; private set; }

        public int AnimMoveY { get; private set; }

        public int AnimMoveX { get; private set; }

        public float DistanceToTarget => Vector2.Distance(this._player.position, this.transform.position);

        public Transform Target => this._player;

        public Vector2 DirectionToTarget => (this.transform.position - this._player.position).normalized;


        private readonly FiniteStateMachine<GreatCreatorState> _fsm = new();

        [SerializeField] private Transform _spawnPos;

        [field: SerializeField] public NavMeshAgent Agent { get; private set; }

        private Transform _player;

        private void Start()
        {
            this.Agent.updateRotation = false;
            this.Agent.updateUpAxis = false;
            this._player = FindFirstObjectByType<Player>().transform;

            this.States.Add(GreatCreatorState.Idle,
                new GCIdleState(this._fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Idle))));
            this.States.Add(GreatCreatorState.Swarm,
                new GCSwarmState(this._fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Swarm))));
            this.States.Add(GreatCreatorState.RunAway, new GCRunAwayState(this._fsm, this, Animator.StringToHash("Walk")));
            this.States.Add(GreatCreatorState.Dash,
                new GCDashState(this._fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Dash))));
            this.States.Add(GreatCreatorState.Transform,
                new GCTransformationState(this._fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Transform))));
            this.States.Add(GreatCreatorState.Spin,
                new GCSpinState(this._fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Spin))));
            this.States.Add(GreatCreatorState.Death,
                new GCDeathState(this._fsm, this, Animator.StringToHash(nameof(GreatCreatorState.Death))));

            this._fsm.Initialize(this.States[GreatCreatorState.Idle]);
            this.Stage = 0;
            this.CurrentHealth = this.MaxHealth;

            this.AnimMoveX = Animator.StringToHash("MoveX");
            this.AnimMoveY = Animator.StringToHash("MoveY");
        }

        private void Update()
        {
            this._fsm.CurrentState.OnFrameUpdate();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<Player>().Knockback.GetKnockedBack(this.transform, 1f);
                if (this._fsm.CurrentState.GetType() == typeof(GCDashState))
                    collision.collider.GetComponent<Player>().TakeDamage(10f);
            }
        }

        public void SetAnimationMoveParameters(Vector2 Move)
        {
            this.Animator.SetFloat(this.AnimMoveX, Move.x);
            this.Animator.SetFloat(this.AnimMoveY, Move.y);
        }

        public void SpawnMinions(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var obj = ObjectPool.Instance.GetPooledObject(Random.Range(0f, 1f) > 0.5f
                    ? PooledObjectType.BombMinion
                    : PooledObjectType.DrillMinion);
                obj.transform.position = (Vector2)this._spawnPos.position + Random.insideUnitCircle * 0.1f;
                obj.SetActive(true);
            }
        }


        public void TakeDamage(float damage)
        {
            this.CurrentHealth = Mathf.Max(this.CurrentHealth - damage, 0);
            this.GetComponent<HitFlash>().HitFlashRoutine();

            if (this.Stage < this.StageTransitionHealthPercentage.Count && this.CurrentHealth < this.MaxHealth * this.StageTransitionHealthPercentage[this.Stage])
            {
                this.Stage++;
                if (this.Stage == 1)
                {
                    this._fsm.SwitchState(this.States[GreatCreatorState.Transform]);
                    ((GCRunAwayState)this.States[GreatCreatorState.RunAway]).SetAnimation(
                        Animator.StringToHash("WalkPhase2FourArms"));
                    ((GCSpinState)this.States[GreatCreatorState.Swarm]).SetAnimation(Animator.StringToHash("Spin"));
                }

                if (this.Stage == 2)
                {
                    ((GCRunAwayState)this.States[GreatCreatorState.RunAway]).SetAnimation(
                        Animator.StringToHash("WalkPhase2ThreeArms"));
                    this._fsm.SwitchState(this.States[GreatCreatorState.RunAway]);
                }

                if (this.Stage == 3)
                {
                    ((GCRunAwayState)this.States[GreatCreatorState.RunAway]).SetAnimation(
                        Animator.StringToHash("WalkPhase2TwoArms"));
                    this._fsm.SwitchState(this.States[GreatCreatorState.RunAway]);
                }
            }

            if (this.CurrentHealth == 0) Elevator.Instance.ActivateElevator();

            this.OnHitTaken?.Invoke();
        }

        public IEnumerator ShootProjectiles()
        {
            var start = Vector2.up;
            for (var i = 0; i < 12; i++)
            {
                var laser = ObjectPool.Instance.GetPooledObject(PooledObjectType.LaserProjectile);
                laser.GetComponent<Projectile>().SetUnaffectedLayer(LayerMask.NameToLayer("Enemy"));
                laser.transform.position = this.transform.position;

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
                laser.transform.position = this.transform.position;
                laser.GetComponent<Rigidbody2D>().linearVelocity = start * 5;
                start = Quaternion.AngleAxis(30, Vector3.forward) * start;
            }
        }
    }
}
