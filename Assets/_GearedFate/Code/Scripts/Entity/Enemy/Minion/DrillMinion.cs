// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// DrillMinion is a class that will control the behavior of the Drill Minion enemy.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class DrillMinion : BossMinion
    {
        [Header("Drill Minion Stats")]
        [SerializeField]
        private float touchDmgAmt = 5.0f;

        private Animator _animator; // Blaze added this line
        private DrillMinionState _currentState = DrillMinionState.Idle;
        private float _stateTimer = 0.0f;
        private float _startingSpeed; // Blaze added this line

        protected enum DrillMinionState
        {
            Idle,
            MoveIntoPosition,
            Charge,
            Retreat,
            Dying,
        }

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>(); // Blaze added this line
            _startingSpeed = Agent.speed; // Blaze added this line
        }

        protected override void Update()
        {
            switch (_currentState)
            {
                case DrillMinionState.Idle:
                    Idle();
                    break;
                case DrillMinionState.MoveIntoPosition:
                    MoveIntoPosition();
                    break;
                case DrillMinionState.Charge:
                    Attack();
                    break;
                case DrillMinionState.Retreat:
                    Retreat();
                    break;
                case DrillMinionState.Dying:
                    break;
            }
        }

        protected void Idle()
        {
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= AttackCooldown)
            {
                _currentState = DrillMinionState.Charge;
                _stateTimer = 0.0f;
            }
        }

        protected void MoveIntoPosition()
        {
            MoveToTarget(Target);
            if (Agent.remainingDistance <= 10)
            {
                _currentState = DrillMinionState.Charge;
                Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                Agent.speed *= 2;
            }
        }

        protected override void Attack()
        {
            MoveToTarget(Target);
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= 2)
            {
                _currentState = DrillMinionState.Idle;
                _stateTimer = 0.0f;
                Agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

                // agent.speed /= 2; // Blaze commented this line
                Agent.speed = _startingSpeed; // Blaze adden this line
            }
        }

        protected void Retreat()
        {
            // TODO: move away from player for a bit
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= 4)
            {
                _currentState = DrillMinionState.Idle;
                _stateTimer = 0.0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_currentState != DrillMinionState.Charge)
            {
                return;
            }

            if (other.TryGetComponent(out Player player))
            {
                if (player.IsInvulnerable)
                {
                    return;
                }

                _animator.SetTrigger("Attack"); // Blaze added this line
                _currentState = DrillMinionState.Retreat;
                Agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                Agent.speed *= 2;
                player.TakeDamage(touchDmgAmt);
                player.GetComponent<Knockback>().GetKnockedBack(transform, KnockBackAmt); // Blaze added this line
            }
        }
    }
}
