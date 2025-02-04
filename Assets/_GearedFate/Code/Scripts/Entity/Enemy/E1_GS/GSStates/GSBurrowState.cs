//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using System.Linq;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GSBurrowState
    /// </summary>
    public class GSBurrowState : GSBaseState
    {
        public float BurrowTimer = 99f;
        public bool Animating = true;
        private bool isActive = false;
        private float initialVolume;

        public GSBurrowState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            this.initialVolume = this.gearboundSentinel.AudioSource.volume;
            this.isActive = true;
            this.gearboundSentinel.StartCoroutine(this.StartBurrow());

            this.BurrowTimer = 6f;
            base.OnEnter();
        }

        // quick and dirty
        private IEnumerator DiggingSounds()
        {
            this.gearboundSentinel.AudioSource.volume = this.initialVolume * 0.5f;
            while (!this.Animating && this.isActive)
            {
                this.gearboundSentinel.AudioSource.clip = this.gearboundSentinel.AudioBurrowing;
                this.gearboundSentinel.AudioSource.pitch = Random.Range(0.75f, 0.8f);

                this.gearboundSentinel.AudioSource.Play();
                this.gearboundSentinel.AudioSource.time = Random.Range(0.5f, 1.3f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        public override void OnExit()
        {
            this.gearboundSentinel.ResetMoveSpeed();
            this.isActive = false;
            this.gearboundSentinel.AudioSource.volume = this.initialVolume;
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();

            if (this.Animating) // don't move or play moving sounds when intro/outro animation are playing. Also prevents it calling EndBurrow repeatedly
            {
                return;
            }

            this.BurrowTimer -= Time.fixedDeltaTime;
            if (this.BurrowTimer <= 0f)
            {
                this.BurrowTimer = 99f; // Reset in case OnPhysicsUpdate can be called before OnEnter
                this.gearboundSentinel.StartCoroutine(this.EndBurrow());
                return;
            }

            this.gearboundSentinel.NavMeshAgent.SetDestination(this.gearboundSentinel.TargetPlayer.transform.position);
            var diffToTarget = this.gearboundSentinel.TargetPlayer.transform.position - this.gearboundSentinel.transform.position;
            const float popUpDistance = 0.1f;
            if (diffToTarget.sqrMagnitude < popUpDistance * popUpDistance)
            {
                this.gearboundSentinel.StartCoroutine(this.EndBurrow());
                return;
            }
        }

        public IEnumerator StartBurrow()
        {
            this.Animating = true;
            if (this.gearboundSentinel.AudioBurrow)
            {
                this.gearboundSentinel.AudioSource.PlayOneShot(this.gearboundSentinel.AudioBurrow);
            }

            this.gearboundSentinel.CurSpeed = 0f;
            this.gearboundSentinel.Animator.speed = 1f;
            this.gearboundSentinel.Animator.Play("Burrow", -1, 0f);
            yield return null; // TODO: Can we remove this?
            yield return new WaitForSeconds(
                this.gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length -
                                            0.1f);
            // disable collider just before animation finishes
            this.gearboundSentinel.DisableColliders();
            this.gearboundSentinel.Shadow.SetActive(false);
            this.gearboundSentinel.ResetMoveSpeed();
            this.gearboundSentinel.CurSpeed *= this.gearboundSentinel.Data.BurrowSpeedMultiplier;
            yield return new WaitForSeconds(0.1f);
            this.Animating = false;
            this.gearboundSentinel.StartCoroutine(this.DiggingSounds());
            this.gearboundSentinel.Animator.Play("Burrowed", -1, 0f);
        }

        public IEnumerator EndBurrow()
        {
            this.gearboundSentinel.AudioSource.pitch = 1f;
            if (this.gearboundSentinel.AudioUnBurrow)
            {
                this.gearboundSentinel.AudioSource.PlayOneShot(this.gearboundSentinel.AudioUnBurrow);
            }

            this.Animating = true;
            this.gearboundSentinel.Animator.speed = 2f; // play faster until the hit
            this.gearboundSentinel.CurSpeed = 0f;
            this.gearboundSentinel.Animator.Play("UnBurrow", -1, 0f);
            const float HitTimeAfterUnburrowStart = 0.1f;
            const float CollisionTimeBeforeUnburrowEnd = 0.4f;

            // wait a short moment before dealing damage
            yield return new WaitForSeconds(HitTimeAfterUnburrowStart);
            this.gearboundSentinel.Shadow.SetActive(true);
            this.gearboundSentinel.Animator.speed = 1f;
            var hits = Physics2D
                .OverlapCircleAll(
                    this.gearboundSentinel.transform.position, 1.5f,
                    LayerMask.GetMask("Player")) // todo: config radius
                .Where(x => !x.isTrigger); // the player's feet
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    player.TakeDamage(this.gearboundSentinel.Data.BurrowDamage);
                }
            }

            // wait until just before animation end to enable collisions
            yield return new WaitForSeconds(
                this.gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length -
                                            HitTimeAfterUnburrowStart - CollisionTimeBeforeUnburrowEnd);
            this.gearboundSentinel.EnableColliders();

            // wait the last bit of the animation
            yield return new WaitForSeconds(CollisionTimeBeforeUnburrowEnd);
            // leave Animating true for next time in case OnPhysicsUpdate gets called before OnEnter

            this.gearboundSentinel.ResetMoveSpeed();
            // in the last phase, GS throws bombs when he pops up
            if (this.gearboundSentinel.Phase == 2)
            {
                this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Bomb]);
            }

            this.fsm.SwitchState(this.gearboundSentinel.States[GearboundSentinel.State.Chase]);
        }
    }
}
