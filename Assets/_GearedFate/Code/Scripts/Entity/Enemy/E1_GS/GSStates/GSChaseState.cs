// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using DayenCreation;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// GSChaseState
    /// </summary>
    public class GSChaseState : GSBaseState
    {
        private const float RunAwayDistance = 3f;

        private Vector3 _targetPos;
        private float _attackCooldown = 99f; // only counts down from this state
        private int _circlingDirection = -1;
        private bool _isActive;
        private bool _wasMoving;
        private Coroutine _switchCirclingCoroutine;

        public GSChaseState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs)
            : base(fsm, state, gs)
        {
            _switchCirclingCoroutine ??= gs.StartCoroutine(SwitchCirlingDir(0.5f, 3f));
        }

        public override void OnEnter()
        {
            _isActive = true;
            _wasMoving = false;
            _attackCooldown = GearboundSentinel.Phase switch
            {
                0 => 5f,
                1 => 3.5f,
                _ => 1f
            };
            GearboundSentinel.Animator.speed = 1f;
            GearboundSentinel.Animator.Play("Idle");

            // Coroutines stop themselves when isActive gets set to false by OnExit
            GearboundSentinel.StartCoroutine(MoveToTarget());
            GearboundSentinel.StartCoroutine(AnimateAndSound());
            base.OnEnter();
        }

        public override void OnExit()
        {
            _isActive = false;

            base.OnExit();
        }

        public override void OnPhysicsUpdate()
        {
            if (GearboundSentinel.TargetPlayer == null)
            {
                GearboundSentinel.TargetPlayer = Object.FindObjectsByType<Player>(FindObjectsSortMode.None).Random();
            }

            if (GearboundSentinel.TargetPlayer == null)
            {
                Debug.LogError("There is no TargetPlayer set on " + nameof(GearboundSentinel));
                return;
            }

            var vecToTarget = GearboundSentinel.TargetPlayer.transform.position - GearboundSentinel.transform.position;
            var distToTarget = vecToTarget.magnitude;

            UpdateDestination(vecToTarget, distToTarget);

            _attackCooldown -= Time.fixedDeltaTime;
            if (_attackCooldown < 0f)
            {
                SelectAttack(distToTarget);
            }

            base.OnPhysicsUpdate();

            // TODO: Switch to attack/burrow states based on distance, timers, etc.
        }

        private void UpdateDestination(Vector3 diffToTarget, float distToTarget)
        {
            const float distanceMargin = 1f;

            // if too far from target, move to them
            if (distToTarget > GearboundSentinel.CurDistanceGoal + distanceMargin)
            {
                _targetPos = GearboundSentinel.TargetPlayer.transform.position;
            }

            // else if too close to target, run away from them
            else if (distToTarget < GearboundSentinel.CurDistanceGoal - distanceMargin)
            {
                var runAwayVec = -diffToTarget.normalized * RunAwayDistance;
                _targetPos = GearboundSentinel.transform.position + runAwayVec;
            }

            // we're within margin of distance goal
            else
            {
                // rotate target position 30 deg around the player, using circlingDirection for clockwise or ccw
                var vecFromPlayer = GearboundSentinel.transform.position - GearboundSentinel.TargetPlayer.transform.position;
                var rotatedVec = Quaternion.AngleAxis(_circlingDirection * 30f, Vector3.forward) * vecFromPlayer;
                _targetPos = GearboundSentinel.TargetPlayer.transform.position + rotatedVec;
            }
        }

        private IEnumerator MoveToTarget()
        {
            while (_isActive)
            {
                Vector2 targetVector = Vector3.zero;
                NavMeshPath navMeshPath = new ();

                // TODO: area mask
                if (NavMesh.SamplePosition(_targetPos, out var hit, RunAwayDistance, GearboundSentinel.NavMeshAgent.areaMask))
                {
                    _targetPos = hit.position;
                    if (GearboundSentinel.NavMeshAgent.CalculatePath(_targetPos, navMeshPath) &&
                        navMeshPath.corners.Length > 1)
                    {
                        targetVector = navMeshPath.corners[1] - GearboundSentinel.transform.position;
                        targetVector = targetVector.SnapToCardinal(true);
                    }
                    else
                    {
                        Debug.LogError("Path failed!");
                    }
                }
                else
                {
                    Debug.LogError("Path failed!");
                }

                var wait = Random.Range(0.3f, 0.8f);
                while (wait > 0f && _isActive)
                {
                    const float
                        vecMultiplier =
                            2f; // scales the movement vector to give it a little buffer; seems to help for some reason
                    var newDest = (Vector2)GearboundSentinel.transform.position + (targetVector *
                        (GearboundSentinel.CurSpeed * Time.fixedDeltaTime * vecMultiplier));
                    var success = GearboundSentinel.NavMeshAgent.SetDestination(newDest);
                    if (!success)
                    {
                        Debug.LogError("Failed?");
                    }

                    yield return new WaitForFixedUpdate();
                    wait -= Time.fixedDeltaTime;
                }
            }
        }

        private IEnumerator AnimateAndSound()
        {
            while (_isActive)
            {
                var speed = GearboundSentinel.NavMeshAgent.velocity.magnitude;
                const float idleSpeedThreshold = 0.2f;
                if (speed > idleSpeedThreshold)
                {
                    if (!_wasMoving)
                    {
                        _wasMoving = true;
                        GearboundSentinel.Animator.Play("Walk Blend Tree");
                    }

                    if ((!GearboundSentinel.AudioSource.isPlaying || GearboundSentinel.AudioSource.clip != GearboundSentinel.AudioMovement) && GearboundSentinel.AudioMovement != null)
                    {
                        GearboundSentinel.AudioSource.clip = GearboundSentinel.AudioMovement;
                        GearboundSentinel.AudioSource.pitch = GearboundSentinel.CurSpeed;
                        GearboundSentinel.AudioSource.Play();
                    }

                    // TODO: remove dividing constant and design the animation for 1u/s?
                    GearboundSentinel.Animator.speed = GearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2f * speed;
                }
                else
                {
                    if (_wasMoving)
                    {
                        _wasMoving = false;
                        GearboundSentinel.Animator.Play("Idle");
                        GearboundSentinel.Animator.speed = 1f;
                    }
                }

                yield return null; // frame
            }
        }

        private void SelectAttack(float distToTarget)
        {
            switch (GearboundSentinel.Phase)
            {
                case 0:
                    // fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Bomb]); break; // TODO: Comment out whole line. This is for testing.
                    // random between bombs and shoot (shotgun spray)
                    Fsm.SwitchState(
                        GearboundSentinel[
                        CollectionExtensions.SelectRandom(
                            GearboundSentinel.State.Bomb,
                            GearboundSentinel.State.Shoot)]);
                    break;
                case 1:
                    // choose randomly between burrow or attack (bomb or shoot)
                    if (Random.Range(0, 2) == 0)
                    {
                        Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Burrow]);
                    }
                    else
                    {
                        // choose randomly whether to attack based on distance or random
                        // (because random has a 50% chance of choosing the same as choosing based on distance, this works out to a 75% chance of choosing based on distance and a 25% chance to do the opposite)
                        if (Random.Range(0, 2) == 0)
                        {
                            // if far away, throw bombs; else, shoot
                            if (distToTarget > 4f)
                            {
                                Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Bomb]);
                            }
                            else
                            {
                                Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Shoot]);
                            }
                        }
                        else
                        {
                            Fsm.SwitchState(
                                GearboundSentinel[
                                CollectionExtensions.SelectRandom(
                                    GearboundSentinel.State.Bomb,
                                    GearboundSentinel.State.Shoot)]);
                        }
                    }

                    break;
                default: // last phase
                    // Random between bombs or shoot, with shoot being twice as likely (360 spray transitions to burrow when done)
                    Fsm.SwitchState(
                        GearboundSentinel[
                        CollectionExtensions.SelectRandom(
                            GearboundSentinel.State.Bomb,
                            GearboundSentinel.State.Shoot,
                            GearboundSentinel.State.Shoot)]);
                    break;
            }
        }

        private IEnumerator SwitchCirlingDir(float minCD, float maxCD)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minCD, maxCD));
                _circlingDirection = -_circlingDirection;
            }
        }
    }
}
