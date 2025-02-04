//
// Copyright (c) BTG. All rights reserved.
//

using DayenCreation;
using System.Collections;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GSBombState
    /// </summary>
    public class GSBombState : GSBaseState
    {
        public GSBombState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            this.gearboundSentinel.CurSpeed = 0f;
            base.OnEnter();
            this.gearboundSentinel.StartCoroutine(this.ThrowBombs());
        }

        public IEnumerator ThrowBombs()
        {
            var timeBetweenBombs = this.gearboundSentinel.Phase switch // TODO: Make configurable
            {
                0 => 0.6f,
                1 => 0.4f,
                _ => 0.15f
            };
            var veryFast = timeBetweenBombs < 0.25f;
            this.gearboundSentinel.Animator.speed = 1f;
            this.gearboundSentinel.Animator.Play("Bomb Throw Blend Tree", -1, 0f);
            yield return null;
            var animationLength = this.gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            var time = 0f;
            if (veryFast)
            {
                yield return new WaitForSeconds(animationLength / 2f);
                time += animationLength / 2f;
            }

            if (!veryFast)
            {
                this.gearboundSentinel.Animator.speed = this.gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / timeBetweenBombs;
            }

            for (var i = 0; i < 3 + this.gearboundSentinel.Phase; i++) // 3, 4, 5
            {
                // pick a random direction's bomb position as the origin rather than having to do maths or consistently being off in the same direction.
                var shotCalculationOrigin = CollectionExtensions.SelectRandom(
                    this.gearboundSentinel.BombUpPos,
                    this.gearboundSentinel.BombRightPos,
                    this.gearboundSentinel.BombDownPos,
                    this.gearboundSentinel.BombLeftPos);
                Vector2 vec = this.gearboundSentinel.TargetPlayer.transform.position - shotCalculationOrigin.position
                              + new Vector3(Random.Range(-2f, 2f),
                                  Random.Range(-2f, 2f)); // add some randomness to the target position
                this.gearboundSentinel.Animator.SetFloat("AttackDirX", vec.x);
                this.gearboundSentinel.Animator.SetFloat("AttackDirY", vec.y);

                // if almost instant, resume at current time but re-run the blend tree to rotate correctly. If not instant, restart the throw animation
                this.gearboundSentinel.Animator.Play("Bomb Throw Blend Tree", -1, veryFast ? time / animationLength : 0f);
                yield return new WaitForSeconds(timeBetweenBombs);
                if (this.gearboundSentinel.AudioBombThrow != null)
                {
                    this.gearboundSentinel.AudioSource.PlayOneShot(
                        this.gearboundSentinel.AudioBombThrow,
                        this.gearboundSentinel.CalculateVolume(timeBetweenBombs));
                }

                var cardinal = vec.SnapToCardinal();
                Transform spawnTransform = null;
                if (cardinal.x > 0)
                {
                    spawnTransform = this.gearboundSentinel.BombRightPos;
                }
                else if (cardinal.x < 0)
                {
                    spawnTransform = this.gearboundSentinel.BombLeftPos;
                }

                if (cardinal.y > 0)
                {
                    spawnTransform = this.gearboundSentinel.BombUpPos;
                }
                else if (cardinal.y < 0)
                {
                    spawnTransform = this.gearboundSentinel.BombDownPos;
                }

                var bomb = Object.Instantiate(
                    this.gearboundSentinel.BombPrefab, spawnTransform.position,
                    Quaternion.identity);
                const float bombSpeedMult = 1.5f; // arbitrary
                if (vec.magnitude > 7f) // arbitrary target distance limit
                {
                    vec = vec.normalized * 7f;
                }

                bomb.GetComponent<Rigidbody2D>().linearVelocity = vec * bombSpeedMult;

                time += timeBetweenBombs;
            }

            this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Chase]);
        }

        public override void OnExit()
        {
            this.gearboundSentinel.ResetMoveSpeed();
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
