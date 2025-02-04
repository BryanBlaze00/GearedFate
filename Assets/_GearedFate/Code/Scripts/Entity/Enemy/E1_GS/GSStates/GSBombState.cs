//
// Copyright (c) BTG. All rights reserved.
//

using DayenCreation;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GSBombState
    /// </summary>
    public class GSBombState : GSBaseState
	{
		
		public GSBombState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs) : base(fsm, state, gs)
		{
		}

		public override void OnEnter()
		{
            gearboundSentinel.CurSpeed = 0f;
            base.OnEnter();
			gearboundSentinel.StartCoroutine(ThrowBombs());
		}

		public IEnumerator ThrowBombs()
		{
            float timeBetweenBombs = gearboundSentinel.Phase switch // TODO: Make configurable
            {
                0 => 0.6f,
                1 => 0.4f,
                _ => 0.15f
            };
            bool veryFast = timeBetweenBombs < 0.25f;
            gearboundSentinel.Animator.speed = 1f;
            gearboundSentinel.Animator.Play("Bomb Throw Blend Tree", -1, 0f);
            yield return null;
            float animationLength = gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            float time = 0f;
            if (veryFast)
            {
                yield return new WaitForSeconds(animationLength / 2f);
                time += animationLength / 2f;
            }
            if (!veryFast)
                gearboundSentinel.Animator.speed = gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / timeBetweenBombs;
            for (int i = 0; i < 3 + gearboundSentinel.Phase; i++) // 3, 4, 5
			{
                // pick a random direction's bomb position as the origin rather than having to do maths or consistently being off in the same direction.
                Transform shotCalculationOrigin = CollectionExtensions.SelectRandom(
                    gearboundSentinel.BombUpPos,
                    gearboundSentinel.BombRightPos,
                    gearboundSentinel.BombDownPos,
                    gearboundSentinel.BombLeftPos);
                Vector2 vec = gearboundSentinel.TargetPlayer.transform.position - shotCalculationOrigin.position 
					+ new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f)); // add some randomness to the target position
                gearboundSentinel.Animator.SetFloat("AttackDirX", vec.x);
                gearboundSentinel.Animator.SetFloat("AttackDirY", vec.y);

                // if almost instant, resume at current time but re-run the blend tree to rotate correctly. If not instant, restart the throw animation
                gearboundSentinel.Animator.Play("Bomb Throw Blend Tree", -1, veryFast ? time / animationLength : 0f);
                yield return new WaitForSeconds(timeBetweenBombs);
                if(gearboundSentinel.AudioBombThrow != null)
                    gearboundSentinel.AudioSource.PlayOneShot(gearboundSentinel.AudioBombThrow, gearboundSentinel.CalculateVolume(timeBetweenBombs));
            var cardinal = vec.SnapToCardinal();
                Transform spawnTransform = null;
                if (cardinal.x > 0)
                    spawnTransform = gearboundSentinel.BombRightPos;
                else if (cardinal.x < 0)
                    spawnTransform = gearboundSentinel.BombLeftPos;
                if (cardinal.y > 0)
                    spawnTransform = gearboundSentinel.BombUpPos;
                else if (cardinal.y < 0)
                    spawnTransform = gearboundSentinel.BombDownPos;
                var bomb = GameObject.Instantiate(gearboundSentinel.BombPrefab, spawnTransform.position, Quaternion.identity);
				const float bombSpeedMult = 1.5f; // arbitrary
				if (vec.magnitude > 7f) // arbitrary target distance limit
					vec = vec.normalized * 7f;
				bomb.GetComponent<Rigidbody2D>().linearVelocity = vec * bombSpeedMult;

                time += timeBetweenBombs;
            }
			fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Chase]);
		}

		public override void OnExit()
		{
            gearboundSentinel.ResetMoveSpeed();
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