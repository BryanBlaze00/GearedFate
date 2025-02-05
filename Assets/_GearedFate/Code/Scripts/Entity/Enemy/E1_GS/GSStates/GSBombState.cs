// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using DayenCreation;
    using UnityEngine;

    /// <summary>
    /// GSBombState
    /// </summary>
    public class GSBombState : GSBaseState
    {
        public GSBombState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs)
            : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            GearboundSentinel.CurSpeed = 0f;
            base.OnEnter();
            GearboundSentinel.StartCoroutine(ThrowBombs());
        }

        public IEnumerator ThrowBombs()
        {
            var timeBetweenBombs = GearboundSentinel.Phase switch // TODO: Make configurable
            {
                0 => 0.6f,
                1 => 0.4f,
                _ => 0.15f
            };
            var veryFast = timeBetweenBombs < 0.25f;
            GearboundSentinel.Animator.speed = 1f;
            GearboundSentinel.Animator.Play("Bomb Throw Blend Tree", -1, 0f);
            yield return null;
            var animationLength = GearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            var time = 0f;

            if (veryFast)
            {
                yield return new WaitForSeconds(animationLength / 2f);
                time += animationLength / 2f;
            }

            if (!veryFast)
            {
                GearboundSentinel.Animator.speed = GearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / timeBetweenBombs;
            }

            for (int i = 0; i < 3 + GearboundSentinel.Phase; i++)
            {
                // pick a random direction's bomb position as the origin rather than having to do maths or consistently being off in the same direction.
                var shotCalculationOrigin = CollectionExtensions.SelectRandom(
                    GearboundSentinel.BombUpPos,
                    GearboundSentinel.BombRightPos,
                    GearboundSentinel.BombDownPos,
                    GearboundSentinel.BombLeftPos);
                Vector2 vec = GearboundSentinel.TargetPlayer.transform.position - shotCalculationOrigin.position
                              + new Vector3(
                                  Random.Range(-2f, 2f),
                                  Random.Range(-2f, 2f)); // add some randomness to the target position
                GearboundSentinel.Animator.SetFloat("AttackDirX", vec.x);
                GearboundSentinel.Animator.SetFloat("AttackDirY", vec.y);

                // if almost instant, resume at current time but re-run the blend tree to rotate correctly. If not instant, restart the throw animation
                GearboundSentinel.Animator.Play("Bomb Throw Blend Tree", -1, veryFast ? time / animationLength : 0f);
                yield return new WaitForSeconds(timeBetweenBombs);
                if (GearboundSentinel.AudioBombThrow != null)
                {
                    GearboundSentinel.AudioSource.PlayOneShot(
                        GearboundSentinel.AudioBombThrow,
                        GearboundSentinel.CalculateVolume(timeBetweenBombs));
                }

                var cardinal = vec.SnapToCardinal();
                Transform spawnTransform = null;
                if (cardinal.x > 0)
                {
                    spawnTransform = GearboundSentinel.BombRightPos;
                }
                else if (cardinal.x < 0)
                {
                    spawnTransform = GearboundSentinel.BombLeftPos;
                }

                if (cardinal.y > 0)
                {
                    spawnTransform = GearboundSentinel.BombUpPos;
                }
                else if (cardinal.y < 0)
                {
                    spawnTransform = GearboundSentinel.BombDownPos;
                }

                var bomb = Object.Instantiate(
                    GearboundSentinel.BombPrefab,
                    spawnTransform.position,
                    Quaternion.identity);

                const float bombSpeedMult = 1.5f; // arbitrary

                // arbitrary target distance limit
                if (vec.magnitude > 7f)
                {
                    vec = vec.normalized * 7f;
                }

                bomb.GetComponent<Rigidbody2D>().linearVelocity = vec * bombSpeedMult;

                time += timeBetweenBombs;
            }

            Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Chase]);
        }

        public override void OnExit()
        {
            GearboundSentinel.ResetMoveSpeed();
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
