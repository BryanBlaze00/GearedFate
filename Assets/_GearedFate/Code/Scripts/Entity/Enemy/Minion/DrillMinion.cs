//
// Copyright (c) BTG. All rights reserved.
//

using System.Numerics;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace BTG
{
    /// <summary>
    /// DrillMinion is a class that will control the behavior of the Drill Minion enemy.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class DrillMinion : BossMinion
    {
        [Header("Drill Minion Stats")] [SerializeField]
        private float touchDmgAmt = 5.0f;

        protected enum DrillMinionState
        {
            Idle,
            MoveIntoPosition,
            Charge,
            Retreat,
            Dying
        }

        private Animator _animator; // Blaze added this line
        protected DrillMinionState _CurrentState = DrillMinionState.Idle;
        private float _stateTimer = 0.0f;
        private float _startingSpeed; // Blaze added this line

        protected override void Awake()
        {
            base.Awake();
            this._animator = this.GetComponent<Animator>(); // Blaze added this line
            this._startingSpeed = this.agent.speed; // Blaze added this line
        }

        protected override void Update()
        {
            switch (this._CurrentState)
            {
                case DrillMinionState.Idle:
                    this.Idle();
                    break;
                case DrillMinionState.MoveIntoPosition:
                    this.MoveIntoPosition();
                    break;
                case DrillMinionState.Charge:
                    this.Attack();
                    break;
                case DrillMinionState.Retreat:
                    this.Retreat();
                    break;
                case DrillMinionState.Dying:
                    break;
            }
        }

        protected void Idle()
        {
            this._stateTimer += Time.deltaTime;
            if (this._stateTimer >= this.attackCooldown)
            {
                this._CurrentState = DrillMinionState.Charge;
                this._stateTimer = 0.0f;
            }
        }

        protected void MoveIntoPosition()
        {
            this.MoveToTarget(this.target);
            if (this.agent.remainingDistance <= 10)
            {
                this._CurrentState = DrillMinionState.Charge;
                this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                this.agent.speed *= 2;
            }
        }

        protected override void Attack()
        {
            this.MoveToTarget(this.target);
            this._stateTimer += Time.deltaTime;
            if (this._stateTimer >= 2)
            {
                this._CurrentState = DrillMinionState.Idle;
                this._stateTimer = 0.0f;
                this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

                // agent.speed /= 2; // Blaze commented this line
                this.agent.speed = this._startingSpeed; // Blaze adden this line
            }
        }

        protected void Retreat()
        {
            //TODO: move away from player for a bit
            this._stateTimer += Time.deltaTime;
            if (this._stateTimer >= 4)
            {
                this._CurrentState = DrillMinionState.Idle;
                this._stateTimer = 0.0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this._CurrentState != DrillMinionState.Charge) return;
            if (other.TryGetComponent(out Player player))
            {
                if (player.isInvulnerable) return;

                this._animator.SetTrigger("Attack"); // Blaze added this line
                this._CurrentState = DrillMinionState.Retreat;
                this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                this.agent.speed *= 2;
                player.TakeDamage(this.touchDmgAmt);
                player.GetComponent<Knockback>().GetKnockedBack(this.transform, this.knockBackAmt); // Blaze added this line
            }
        }
    }
}
