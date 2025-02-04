//
// Copyright (c) BTG. All rights reserved.
//

using DayenCreation;
using System.Collections;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GSShootFlameState
    /// </summary>
    public class GSShootState : GSBaseState
    {
        public GSShootState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            // pick a random direction's eye position as the origin.
            // A bit of randomness rather than having to do maths or consistently being off in the same direction.
            var shotCalculationOrigin = CollectionExtensions.SelectRandom(
                this.gearboundSentinel.EyeShootUpPos,
                this.gearboundSentinel.EyeShootRightPos,
                this.gearboundSentinel.EyeShootDownPos,
                this.gearboundSentinel.EyeShootLeftPos);
            this.gearboundSentinel.StartCoroutine(
                this.SprayCoroutine(
                    this.gearboundSentinel.TargetPlayer.transform.position - shotCalculationOrigin.position,
                    Random.Range(Mathf.Max(0, this.gearboundSentinel.Phase - 1), this.gearboundSentinel.Phase + 1)));
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate(); // TODO: Shoot for some time then go back to Chase
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        public IEnumerator ShootProjectile(Vector2 direction, float windUp, Quaternion angleQuat)
        {
            // TODO: Position shot correctly based on head position
            var cardinal = direction.SnapToCardinal();
            Transform spawnTransform = null;
            if (cardinal.x > 0)
            {
                spawnTransform = this.gearboundSentinel.EyeShootRightPos;
            }
            else if (cardinal.x < 0)
            {
                spawnTransform = this.gearboundSentinel.EyeShootLeftPos;
            }

            if (cardinal.y > 0)
            {
                spawnTransform = this.gearboundSentinel.EyeShootUpPos;
            }
            else if (cardinal.y < 0)
            {
                spawnTransform = this.gearboundSentinel.EyeShootDownPos;
            }

            var GO = ObjectPool.Instance.GetPooledObject(this.gearboundSentinel.ProjectileData.PooledObjectType);
            GO.transform.position = spawnTransform.position;
            GO.transform.rotation = angleQuat;
            var proj = GO.GetComponent<Projectile>();
            proj.SetUnaffectedLayer(this.gearboundSentinel.gameObject.layer);
            GO.SetActive(true);
            var projAnimator = GO.GetComponentInChildren<Animator>();
            projAnimator.Play("DirectionalProj_Windup");
            yield return null; // TODO: Can we remove this?
            var projAnim = projAnimator.GetCurrentAnimatorClipInfo(0);
            if (projAnim.Length > 0)
            {
                projAnimator.speed = projAnim[0].clip.length / windUp;
            }

            yield return new WaitForSeconds(windUp);
            projAnimator.Play("DirectionalProj_Moving");
            GO.GetComponent<Rigidbody2D>().linearVelocity =
                direction.normalized * this.gearboundSentinel.ProjectileData.Speed;
        }

        private IEnumerator SprayCoroutine(Vector2 centerDirection, int phasePattern)
        {
            var numBullets = phasePattern switch
            {
                0 => this.gearboundSentinel.Data.Stage0ShotgunProjCount,
                1 => this.gearboundSentinel.Data.Stage1ShotgunProjCount,
                _ => Mathf.RoundToInt(
                    this.gearboundSentinel.Data.Stage2Spray360ProjPerRotation * this.gearboundSentinel.Data.Stage2Spray360NumRotations)
            };
            var spreadDeg = phasePattern switch
            {
                0 => this.gearboundSentinel.Data.Stage0ShotgunSpreadDegrees,
                1 => this.gearboundSentinel.Data.Stage1ShotgunSpreadDegrees,
                _ => 360 * this.gearboundSentinel.Data.Stage2Spray360NumRotations
            };
            var delayBetweenShots = this.gearboundSentinel.Phase switch // note: switches off phase, not phasePattern
            {
                0 => 0.4f,
                1 => 0.15f, // TODO: config delay between shots for phase 0 and 1
                _ => 1 / this.gearboundSentinel.Data.Stage2Spray360RotationsPerSecond / this.gearboundSentinel.Data.Stage2Spray360ProjPerRotation // seconds per rotation divided by projCount
            };
            //bool circle = Mathf.Approximately(spreadDeg, 360f);
            //if (circle)
            //    // temporarily increase for spread calculations
            //    numBullets++;

            var rotPerShot = spreadDeg / (numBullets - 1);
            var startRot = Vector2.SignedAngle(Vector2.up, centerDirection) - spreadDeg / 2f;
            //if(circle)
            //    // remove temporary increase; the last bullet won't be spawned because that'll be the start of the circle
            //    numBullets--;
            for (var i = 0; i < numBullets; i++)
            {
                var angleQuat = Quaternion.AngleAxis(startRot + rotPerShot * i, Vector3.forward);
                var vec = angleQuat * Vector2.up;
                this.gearboundSentinel.Animator.SetFloat("AttackDirX", vec.x);
                this.gearboundSentinel.Animator.SetFloat("AttackDirY", vec.y);
                this.gearboundSentinel.Animator.Play("Shoot Blend Tree", -1, 0f); // play from the start
                yield return null; // TODO: Can we remove this?
                var wait = delayBetweenShots;
                var baseAnimationLength = this.gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                // x2 because half the duration on this animation, half on the bullet animation
                if (delayBetweenShots < baseAnimationLength * 2f) // speed up the animation if needed
                {
                    this.gearboundSentinel.Animator.speed = baseAnimationLength / delayBetweenShots * 2f;
                }
                else
                {
                    this.gearboundSentinel.Animator.speed = 0f;
                    yield return new WaitForSeconds(wait - baseAnimationLength * 2f);
                    wait = baseAnimationLength / 2f;
                    this.gearboundSentinel.Animator.speed = 1f;
                }

                yield return new WaitForSeconds(wait);
                if (this.gearboundSentinel.AudioLaserShot)
                {
                    this.gearboundSentinel.AudioSource.PlayOneShot(
                        this.gearboundSentinel.AudioLaserShot,
                        this.gearboundSentinel.CalculateVolume(delayBetweenShots));
                }

                yield return this.gearboundSentinel.StartCoroutine(this.ShootProjectile(vec, wait, angleQuat));
            }

            // wait a moment before going to the next state
            yield return new WaitForSeconds(0.5f);
            if (phasePattern >= 2) // go from 360 spray straight into burrow
            {
                this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Burrow]);
            }
            else
            {
                this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Chase]);
            }
        }
    }
}
