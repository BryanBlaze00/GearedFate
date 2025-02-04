// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;

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
            initialVolume = gearboundSentinel.AudioSource.volume;
            isActive = true;
            gearboundSentinel.StartCoroutine(StartBurrow());

            BurrowTimer = 6f;
            base.OnEnter();
        }

        // quick and dirty
        private IEnumerator DiggingSounds()
        {
            gearboundSentinel.AudioSource.volume = initialVolume * 0.5f;
            while (!Animating && isActive)
            {
                gearboundSentinel.AudioSource.clip = gearboundSentinel.AudioBurrowing;
                gearboundSentinel.AudioSource.pitch = Random.Range(0.75f, 0.8f);

                gearboundSentinel.AudioSource.Play();
                gearboundSentinel.AudioSource.time = Random.Range(0.5f, 1.3f);
                yield return new WaitForSeconds(0.5f);
            }
        }

        public override void OnExit()
        {
            gearboundSentinel.ResetMoveSpeed();
            isActive = false;
            gearboundSentinel.AudioSource.volume = initialVolume;
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();

            if (Animating) // don't move or play moving sounds when intro/outro animation are playing. Also prevents it calling EndBurrow repeatedly
            {
                return;
            }

            BurrowTimer -= Time.fixedDeltaTime;
            if (BurrowTimer <= 0f)
            {
                BurrowTimer = 99f; // Reset in case OnPhysicsUpdate can be called before OnEnter
                gearboundSentinel.StartCoroutine(EndBurrow());
                return;
            }

            gearboundSentinel.NavMeshAgent.SetDestination(gearboundSentinel.TargetPlayer.transform.position);
            var diffToTarget = gearboundSentinel.TargetPlayer.transform.position - gearboundSentinel.transform.position;
            const float popUpDistance = 0.1f;
            if (diffToTarget.sqrMagnitude < popUpDistance * popUpDistance)
            {
                gearboundSentinel.StartCoroutine(EndBurrow());
                return;
            }
        }

        public IEnumerator StartBurrow()
        {
            Animating = true;
            if (gearboundSentinel.AudioBurrow)
            {
                gearboundSentinel.AudioSource.PlayOneShot(gearboundSentinel.AudioBurrow);
            }

            gearboundSentinel.CurSpeed = 0f;
            gearboundSentinel.Animator.speed = 1f;
            gearboundSentinel.Animator.Play("Burrow", -1, 0f);
            yield return null; // TODO: Can we remove this?
            yield return new WaitForSeconds(
                gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length -
                                            0.1f);

            // disable collider just before animation finishes
            gearboundSentinel.DisableColliders();
            gearboundSentinel.Shadow.SetActive(false);
            gearboundSentinel.ResetMoveSpeed();
            gearboundSentinel.CurSpeed *= gearboundSentinel.Data.BurrowSpeedMultiplier;
            yield return new WaitForSeconds(0.1f);
            Animating = false;
            gearboundSentinel.StartCoroutine(DiggingSounds());
            gearboundSentinel.Animator.Play("Burrowed", -1, 0f);
        }

        public IEnumerator EndBurrow()
        {
            gearboundSentinel.AudioSource.pitch = 1f;
            if (gearboundSentinel.AudioUnBurrow)
            {
                gearboundSentinel.AudioSource.PlayOneShot(gearboundSentinel.AudioUnBurrow);
            }

            Animating = true;
            gearboundSentinel.Animator.speed = 2f; // play faster until the hit
            gearboundSentinel.CurSpeed = 0f;
            gearboundSentinel.Animator.Play("UnBurrow", -1, 0f);
            const float HitTimeAfterUnburrowStart = 0.1f;
            const float CollisionTimeBeforeUnburrowEnd = 0.4f;

            // wait a short moment before dealing damage
            yield return new WaitForSeconds(HitTimeAfterUnburrowStart);
            gearboundSentinel.Shadow.SetActive(true);
            gearboundSentinel.Animator.speed = 1f;
            var hits = Physics2D
                .OverlapCircleAll(
                    gearboundSentinel.transform.position, 1.5f,
                    LayerMask.GetMask("Player")) // todo: config radius
                .Where(x => !x.isTrigger); // the player's feet
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    player.TakeDamage(gearboundSentinel.Data.BurrowDamage);
                }
            }

            // wait until just before animation end to enable collisions
            yield return new WaitForSeconds(
                gearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length -
                                            HitTimeAfterUnburrowStart - CollisionTimeBeforeUnburrowEnd);
            gearboundSentinel.EnableColliders();

            // wait the last bit of the animation
            yield return new WaitForSeconds(CollisionTimeBeforeUnburrowEnd);

            // leave Animating true for next time in case OnPhysicsUpdate gets called before OnEnter

            gearboundSentinel.ResetMoveSpeed();

            // in the last phase, GS throws bombs when he pops up
            if (gearboundSentinel.Phase == 2)
            {
                fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Bomb]);
            }

            fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Chase]);
        }
    }
}
