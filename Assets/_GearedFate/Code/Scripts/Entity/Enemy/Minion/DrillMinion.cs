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

        protected enum DrillMinionState
        {
            Idle,
            MoveIntoPosition,
            Charge,
            Retreat,
            Dying,
        }

        private Animator _animator; // Blaze added this line
        protected DrillMinionState _CurrentState = DrillMinionState.Idle;
        private float _stateTimer = 0.0f;
        private float _startingSpeed; // Blaze added this line

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>(); // Blaze added this line
            _startingSpeed = agent.speed; // Blaze added this line
        }

        protected override void Update()
        {
            switch (_CurrentState)
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
            if (_stateTimer >= attackCooldown)
            {
                _CurrentState = DrillMinionState.Charge;
                _stateTimer = 0.0f;
            }
        }

        protected void MoveIntoPosition()
        {
            MoveToTarget(target);
            if (agent.remainingDistance <= 10)
            {
                _CurrentState = DrillMinionState.Charge;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                agent.speed *= 2;
            }
        }

        protected override void Attack()
        {
            MoveToTarget(target);
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= 2)
            {
                _CurrentState = DrillMinionState.Idle;
                _stateTimer = 0.0f;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

                // agent.speed /= 2; // Blaze commented this line
                agent.speed = _startingSpeed; // Blaze adden this line
            }
        }

        protected void Retreat()
        {
            // TODO: move away from player for a bit
            _stateTimer += Time.deltaTime;
            if (_stateTimer >= 4)
            {
                _CurrentState = DrillMinionState.Idle;
                _stateTimer = 0.0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_CurrentState != DrillMinionState.Charge)
            {
                return;
            }

            if (other.TryGetComponent(out Player player))
            {
                if (player.isInvulnerable)
                {
                    return;
                }

                _animator.SetTrigger("Attack"); // Blaze added this line
                _CurrentState = DrillMinionState.Retreat;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                agent.speed *= 2;
                player.TakeDamage(touchDmgAmt);
                player.GetComponent<Knockback>().GetKnockedBack(transform, knockBackAmt); // Blaze added this line
            }
        }
    }
}
