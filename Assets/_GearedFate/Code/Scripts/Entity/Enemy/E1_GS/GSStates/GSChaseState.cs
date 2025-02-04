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
        public Vector3 TargetPos;
        public float AttackCooldown = 99f; // only counts down from this state
        private int circlingDirection = -1;
        private bool isActive;
        private bool wasMoving;
        private Coroutine switchCirclingCoroutine;

        public GSChaseState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm, state, gs)
        {
            switchCirclingCoroutine ??= gs.StartCoroutine(SwitchCirlingDir(0.5f, 3f));
        }

        public override void OnEnter()
        {
            isActive = true;
            wasMoving = false;
            AttackCooldown = gearboundSentinel.Phase switch
            {
                0 => 5f,
                1 => 3.5f,
                _ => 1f
            };
            gearboundSentinel.Animator.speed = 1f;
            gearboundSentinel.Animator.Play("Idle");

            // Coroutines stop themselves when isActive gets set to false by OnExit
            gearboundSentinel.StartCoroutine(MoveToTarget());
            gearboundSentinel.StartCoroutine(AnimateAndSound());
            base.OnEnter();
        }

        public override void OnExit()
        {
            isActive = false;

            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            if (gearboundSentinel.TargetPlayer == null)
            {
                gearboundSentinel.TargetPlayer = Object.FindObjectsByType<Player>(FindObjectsSortMode.None).Random();
            }

            if (gearboundSentinel.TargetPlayer == null)
            {
                Debug.LogError("There is no TargetPlayer set on " + nameof(gearboundSentinel));
                return;
            }

            var vecToTarget = gearboundSentinel.TargetPlayer.transform.position - gearboundSentinel.transform.position;
            var distToTarget = vecToTarget.magnitude;

            UpdateDestination(vecToTarget, distToTarget);

            AttackCooldown -= Time.fixedDeltaTime;
            if (AttackCooldown < 0f)
            {
                SelectAttack(distToTarget);
            }

            base.OnPhysicsUpdate();

            // TODO: Switch to attack/burrow states based on distance, timers, etc.
        }

        private const float runAwayDistance = 3f;

        private void UpdateDestination(Vector3 diffToTarget, float distToTarget)
        {
            const float distanceMargin = 1f;

            // if too far from target, move to them
            if (distToTarget > gearboundSentinel.CurDistanceGoal + distanceMargin)
            {
                TargetPos = gearboundSentinel.TargetPlayer.transform.position;
            }

            // else if too close to target, run away from them
            else if (distToTarget < gearboundSentinel.CurDistanceGoal - distanceMargin)
            {
                var runAwayVec = -diffToTarget.normalized * runAwayDistance;
                TargetPos = gearboundSentinel.transform.position + runAwayVec;
            }
            else // we're within margin of distance goal
            {
                // rotate target position 30 deg around the player, using circlingDirection for clockwise or ccw
                var vecFromPlayer = gearboundSentinel.transform.position - gearboundSentinel.TargetPlayer.transform.position;
                var rotatedVec = Quaternion.AngleAxis(circlingDirection * 30f, Vector3.forward) * vecFromPlayer;
                TargetPos = gearboundSentinel.TargetPlayer.transform.position + rotatedVec;
            }
        }

        private IEnumerator MoveToTarget()
        {
            while (isActive)
            {
                Vector2 targetVector = Vector3.zero;
                NavMeshPath navMeshPath = new ();
                if (NavMesh.SamplePosition(
                        TargetPos, out var hit, runAwayDistance,
                        gearboundSentinel.NavMeshAgent.areaMask)) //TODO: area mask
                {
                    TargetPos = hit.position;
                    if (gearboundSentinel.NavMeshAgent.CalculatePath(TargetPos, navMeshPath) &&
                        navMeshPath.corners.Length > 1)
                    {
                        targetVector = navMeshPath.corners[1] - gearboundSentinel.transform.position;
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
                while (wait > 0f && isActive)
                {
                    const float
                        vecMultiplier =
                            2f; // scales the movement vector to give it a little buffer; seems to help for some reason
                    var newDest = (Vector2)gearboundSentinel.transform.position + (targetVector *
                        (gearboundSentinel.CurSpeed * Time.fixedDeltaTime * vecMultiplier));
                    var success = gearboundSentinel.NavMeshAgent.SetDestination(newDest);
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
            while (isActive)
            {
                var speed = gearboundSentinel.NavMeshAgent.velocity.magnitude;
                const float idleSpeedThreshold = 0.2f;
                if (speed > idleSpeedThreshold)
                {
                    if (!wasMoving)
                    {
                        wasMoving = true;
                        gearboundSentinel.Animator.Play("Walk Blend Tree");
                    }

                    if ((!gearboundSentinel.AudioSource.isPlaying || gearboundSentinel.AudioSource.clip != gearboundSentinel.AudioMovement) && gearboundSentinel.AudioMovement != null)
                    {
                        gearboundSentinel.AudioSource.clip = gearboundSentinel.AudioMovement;
                        gearboundSentinel.AudioSource.pitch = gearboundSentinel.CurSpeed;
                        gearboundSentinel.AudioSource.Play();
                    }

                    // TODO: remove dividing constant and design the animation for 1u/s?
                    gearboundSentinel.Animator.speed = gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2f * speed;
                }
                else
                {
                    if (wasMoving)
                    {
                        wasMoving = false;
                        gearboundSentinel.Animator.Play("Idle");
                        gearboundSentinel.Animator.speed = 1f;
                    }
                }

                yield return null; // frame
            }
        }

        private void SelectAttack(float distToTarget)
        {
            switch (gearboundSentinel.Phase)
            {
                case 0:
                    //fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Bomb]); break; // TODO: Comment out whole line. This is for testing.
                    // random between bombs and shoot (shotgun spray)
                    fsm.SwitchState(
                        gearboundSentinel.States[
                        CollectionExtensions.SelectRandom(
                            GearboundSentinel.State.Bomb,
                            GearboundSentinel.State.Shoot)]);
                    break;
                case 1:
                    // choose randomly between burrow or attack (bomb or shoot)
                    if (Random.Range(0, 2) == 0)
                    {
                        fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Burrow]);
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
                                fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Bomb]);
                            }
                            else
                            {
                                fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Shoot]);
                            }
                        }
                        else
                        {
                            fsm.SwitchState(
                                gearboundSentinel.States[
                                CollectionExtensions.SelectRandom(
                                    GearboundSentinel.State.Bomb,
                                    GearboundSentinel.State.Shoot)]);
                        }
                    }

                    break;
                default: // last phase
                    // Random between bombs or shoot, with shoot being twice as likely (360 spray transitions to burrow when done)
                    fsm.SwitchState(
                        gearboundSentinel.States[
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
                circlingDirection = -circlingDirection;
            }
        }
    }
}
