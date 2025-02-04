//
// Copyright (c) BTG. All rights reserved.
//

using DayenCreation;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BTG
{
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
            this.switchCirclingCoroutine ??= gs.StartCoroutine(this.SwitchCirlingDir(0.5f, 3f));
        }

        public override void OnEnter()
        {
            this.isActive = true;
            this.wasMoving = false;
            this.AttackCooldown = this.gearboundSentinel.Phase switch
            {
                0 => 5f,
                1 => 3.5f,
                _ => 1f
            };
            this.gearboundSentinel.Animator.speed = 1f;
            this.gearboundSentinel.Animator.Play("Idle");
            // Coroutines stop themselves when isActive gets set to false by OnExit
            this.gearboundSentinel.StartCoroutine(this.MoveToTarget());
            this.gearboundSentinel.StartCoroutine(this.AnimateAndSound());
            base.OnEnter();
        }

        public override void OnExit()
        {
            this.isActive = false;

            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            if (this.gearboundSentinel.TargetPlayer == null)
            {
                this.gearboundSentinel.TargetPlayer = Object.FindObjectsByType<Player>(FindObjectsSortMode.None).Random();
            }

            if (this.gearboundSentinel.TargetPlayer == null)
            {
                Debug.LogError("There is no TargetPlayer set on " + nameof(this.gearboundSentinel));
                return;
            }

            var vecToTarget = this.gearboundSentinel.TargetPlayer.transform.position - this.gearboundSentinel.transform.position;
            var distToTarget = vecToTarget.magnitude;

            this.UpdateDestination(vecToTarget, distToTarget);

            this.AttackCooldown -= Time.fixedDeltaTime;
            if (this.AttackCooldown < 0f)
            {
                this.SelectAttack(distToTarget);
            }

            base.OnPhysicsUpdate();

            // TODO: Switch to attack/burrow states based on distance, timers, etc.
        }

        private const float runAwayDistance = 3f;

        private void UpdateDestination(Vector3 diffToTarget, float distToTarget)
        {
            const float distanceMargin = 1f;
            // if too far from target, move to them
            if (distToTarget > this.gearboundSentinel.CurDistanceGoal + distanceMargin)
            {
                this.TargetPos = this.gearboundSentinel.TargetPlayer.transform.position;
            }
            // else if too close to target, run away from them
            else if (distToTarget < this.gearboundSentinel.CurDistanceGoal - distanceMargin)
            {
                var runAwayVec = -diffToTarget.normalized * runAwayDistance;
                this.TargetPos = this.gearboundSentinel.transform.position + runAwayVec;
            }
            else // we're within margin of distance goal
            {
                // rotate target position 30 deg around the player, using circlingDirection for clockwise or ccw
                var vecFromPlayer = this.gearboundSentinel.transform.position - this.gearboundSentinel.TargetPlayer.transform.position;
                var rotatedVec = Quaternion.AngleAxis(this.circlingDirection * 30f, Vector3.forward) * vecFromPlayer;
                this.TargetPos = this.gearboundSentinel.TargetPlayer.transform.position + rotatedVec;
            }
        }

        private IEnumerator MoveToTarget()
        {
            while (this.isActive)
            {
                Vector2 targetVector = Vector3.zero;
                NavMeshPath navMeshPath = new();
                if (NavMesh.SamplePosition(
                        this.TargetPos, out var hit, runAwayDistance,
                        this.gearboundSentinel.NavMeshAgent.areaMask)) //TODO: area mask
                {
                    this.TargetPos = hit.position;
                    if (this.gearboundSentinel.NavMeshAgent.CalculatePath(this.TargetPos, navMeshPath) &&
                        navMeshPath.corners.Length > 1)
                    {
                        targetVector = navMeshPath.corners[1] - this.gearboundSentinel.transform.position;
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
                while (wait > 0f && this.isActive)
                {
                    const float
                        vecMultiplier =
                            2f; // scales the movement vector to give it a little buffer; seems to help for some reason
                    var newDest = (Vector2)this.gearboundSentinel.transform.position + targetVector *
                        (this.gearboundSentinel.CurSpeed * Time.fixedDeltaTime * vecMultiplier);
                    var success = this.gearboundSentinel.NavMeshAgent.SetDestination(newDest);
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
            while (this.isActive)
            {
                var speed = this.gearboundSentinel.NavMeshAgent.velocity.magnitude;
                const float idleSpeedThreshold = 0.2f;
                if (speed > idleSpeedThreshold)
                {
                    if (!this.wasMoving)
                    {
                        this.wasMoving = true;
                        this.gearboundSentinel.Animator.Play("Walk Blend Tree");
                    }

                    if ((!this.gearboundSentinel.AudioSource.isPlaying || this.gearboundSentinel.AudioSource.clip != this.gearboundSentinel.AudioMovement) && this.gearboundSentinel.AudioMovement != null)
                    {
                        this.gearboundSentinel.AudioSource.clip = this.gearboundSentinel.AudioMovement;
                        this.gearboundSentinel.AudioSource.pitch = this.gearboundSentinel.CurSpeed;
                        this.gearboundSentinel.AudioSource.Play();
                    }

                    // TODO: remove dividing constant and design the animation for 1u/s?
                    this.gearboundSentinel.Animator.speed = this.gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2f * speed;
                }
                else
                {
                    if (this.wasMoving)
                    {
                        this.wasMoving = false;
                        this.gearboundSentinel.Animator.Play("Idle");
                        this.gearboundSentinel.Animator.speed = 1f;
                    }
                }

                yield return null; // frame
            }
        }

        private void SelectAttack(float distToTarget)
        {
            switch (this.gearboundSentinel.Phase)
            {
                case 0:
                    //fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Bomb]); break; // TODO: Comment out whole line. This is for testing.
                    // random between bombs and shoot (shotgun spray)
                    this.fsm.SwitchState(
                        this.gearboundSentinel.States[
                        CollectionExtensions.SelectRandom(
                            GearboundSentinel.State.Bomb,
                            GearboundSentinel.State.Shoot)]);
                    break;
                case 1:
                    // choose randomly between burrow or attack (bomb or shoot)
                    if (Random.Range(0, 2) == 0)
                    {
                        this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Burrow]);
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
                                this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Bomb]);
                            }
                            else
                            {
                                this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Shoot]);
                            }
                        }
                        else
                        {
                            this.fsm.SwitchState(
                                this.gearboundSentinel.States[
                                CollectionExtensions.SelectRandom(
                                    GearboundSentinel.State.Bomb,
                                    GearboundSentinel.State.Shoot)]);
                        }
                    }

                    break;
                default: // last phase
                    // Random between bombs or shoot, with shoot being twice as likely (360 spray transitions to burrow when done)
                    this.fsm.SwitchState(
                        this.gearboundSentinel.States[
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
                this.circlingDirection = -this.circlingDirection;
            }
        }
    }
}
