// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using DayenCreation;
    using UnityEngine;

    /// <summary>
    /// GSShootFlameState
    /// </summary>
    public class GSShootState : GSBaseState
    {
        public GSShootState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs)
            : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // pick a random direction's eye position as the origin.
            // A bit of randomness rather than having to do maths or consistently being off in the same direction.
            var shotCalculationOrigin = CollectionExtensions.SelectRandom(
                GearboundSentinel.EyeShootUpPos,
                GearboundSentinel.EyeShootRightPos,
                GearboundSentinel.EyeShootDownPos,
                GearboundSentinel.EyeShootLeftPos);
            GearboundSentinel.StartCoroutine(
                SprayCoroutine(
                    GearboundSentinel.TargetPlayer.transform.position - shotCalculationOrigin.position,
                    Random.Range(Mathf.Max(0, GearboundSentinel.Phase - 1), GearboundSentinel.Phase + 1)));
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
                spawnTransform = GearboundSentinel.EyeShootRightPos;
            }
            else if (cardinal.x < 0)
            {
                spawnTransform = GearboundSentinel.EyeShootLeftPos;
            }

            if (cardinal.y > 0)
            {
                spawnTransform = GearboundSentinel.EyeShootUpPos;
            }
            else if (cardinal.y < 0)
            {
                spawnTransform = GearboundSentinel.EyeShootDownPos;
            }

            var go = ObjectPool.Instance.GetPooledObject(GearboundSentinel.ProjectileData.PooledObjectType);
            go.transform.position = spawnTransform.position;
            go.transform.rotation = angleQuat;
            var proj = go.GetComponent<Projectile>();
            proj.SetUnaffectedLayer(GearboundSentinel.gameObject.layer);
            go.SetActive(true);
            var projAnimator = go.GetComponentInChildren<Animator>();
            projAnimator.Play("DirectionalProj_Windup");
            yield return null; // TODO: Can we remove this?
            var projAnim = projAnimator.GetCurrentAnimatorClipInfo(0);
            if (projAnim.Length > 0)
            {
                projAnimator.speed = projAnim[0].clip.length / windUp;
            }

            yield return new WaitForSeconds(windUp);
            projAnimator.Play("DirectionalProj_Moving");
            go.GetComponent<Rigidbody2D>().linearVelocity =
                direction.normalized * GearboundSentinel.ProjectileData.Speed;
        }

        private IEnumerator SprayCoroutine(Vector2 centerDirection, int phasePattern)
        {
            var numBullets = phasePattern switch
            {
                0 => GearboundSentinel.Data.Stage0ShotgunProjCount,
                1 => GearboundSentinel.Data.Stage1ShotgunProjCount,
                _ => Mathf.RoundToInt(
                    GearboundSentinel.Data.Stage2Spray360ProjPerRotation * GearboundSentinel.Data.Stage2Spray360NumRotations)
            };
            var spreadDeg = phasePattern switch
            {
                0 => GearboundSentinel.Data.Stage0ShotgunSpreadDegrees,
                1 => GearboundSentinel.Data.Stage1ShotgunSpreadDegrees,
                _ => 360 * GearboundSentinel.Data.Stage2Spray360NumRotations
            };
            var delayBetweenShots = GearboundSentinel.Phase switch // note: switches off phase, not phasePattern
            {
                0 => 0.4f,
                1 => 0.15f, // TODO: config delay between shots for phase 0 and 1
                _ => 1 / GearboundSentinel.Data.Stage2Spray360RotationsPerSecond / GearboundSentinel.Data.Stage2Spray360ProjPerRotation // seconds per rotation divided by projCount
            };

            // temporarily increase for spread calculations
            var rotPerShot = spreadDeg / (numBullets - 1);
            var startRot = Vector2.SignedAngle(Vector2.up, centerDirection) - (spreadDeg / 2f);

            // remove temporary increase; the last bullet won't be spawned because that'll be the start of the circle
            for (var i = 0; i < numBullets; i++)
            {
                var angleQuat = Quaternion.AngleAxis(startRot + (rotPerShot * i), Vector3.forward);
                var vec = angleQuat * Vector2.up;
                GearboundSentinel.Animator.SetFloat("AttackDirX", vec.x);
                GearboundSentinel.Animator.SetFloat("AttackDirY", vec.y);
                GearboundSentinel.Animator.Play("Shoot Blend Tree", -1, 0f); // play from the start
                yield return null; // TODO: Can we remove this?
                var wait = delayBetweenShots;
                var baseAnimationLength = GearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

                // x2 because half the duration on this animation, half on the bullet animation
                // speed up the animation if needed
                if (delayBetweenShots < baseAnimationLength * 2f)
                {
                    GearboundSentinel.Animator.speed = baseAnimationLength / delayBetweenShots * 2f;
                }
                else
                {
                    GearboundSentinel.Animator.speed = 0f;
                    yield return new WaitForSeconds(wait - (baseAnimationLength * 2f));
                    wait = baseAnimationLength / 2f;
                    GearboundSentinel.Animator.speed = 1f;
                }

                yield return new WaitForSeconds(wait);
                if (GearboundSentinel.AudioLaserShot)
                {
                    GearboundSentinel.AudioSource.PlayOneShot(
                        GearboundSentinel.AudioLaserShot,
                        GearboundSentinel.CalculateVolume(delayBetweenShots));
                }

                yield return GearboundSentinel.StartCoroutine(ShootProjectile(vec, wait, angleQuat));
            }

            // wait a moment before going to the next state
            yield return new WaitForSeconds(0.5f);

            // go from 360 spray straight into burrow
            if (phasePattern >= 2)
            {
                Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Burrow]);
            }
            else
            {
                Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Chase]);
            }
        }
    }
}
