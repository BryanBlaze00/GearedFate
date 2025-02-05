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
        private float _burrowTimer = 99f;
        private bool _animating = true;
        private bool _isActive = false;
        private float _initialVolume;

        public GSBurrowState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs)
            : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            _initialVolume = GearboundSentinel.AudioSource.volume;
            _isActive = true;
            GearboundSentinel.StartCoroutine(StartBurrow());

            _burrowTimer = 6f;
            base.OnEnter();
        }

        public override void OnExit()
        {
            GearboundSentinel.ResetMoveSpeed();
            _isActive = false;
            GearboundSentinel.AudioSource.volume = _initialVolume;
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();

            // don't move or play moving sounds when intro/outro animation are playing. Also prevents it calling EndBurrow repeatedly
            if (_animating)
            {
                return;
            }

            _burrowTimer -= Time.fixedDeltaTime;
            if (_burrowTimer <= 0f)
            {
                _burrowTimer = 99f; // Reset in case OnPhysicsUpdate can be called before OnEnter
                GearboundSentinel.StartCoroutine(EndBurrow());
                return;
            }

            GearboundSentinel.NavMeshAgent.SetDestination(GearboundSentinel.TargetPlayer.transform.position);
            var diffToTarget = GearboundSentinel.TargetPlayer.transform.position - GearboundSentinel.transform.position;
            const float popUpDistance = 0.1f;
            if (diffToTarget.sqrMagnitude < popUpDistance * popUpDistance)
            {
                GearboundSentinel.StartCoroutine(EndBurrow());
                return;
            }
        }

        public IEnumerator StartBurrow()
        {
            _animating = true;
            if (GearboundSentinel.AudioBurrow)
            {
                GearboundSentinel.AudioSource.PlayOneShot(GearboundSentinel.AudioBurrow);
            }

            GearboundSentinel.CurSpeed = 0f;
            GearboundSentinel.Animator.speed = 1f;
            GearboundSentinel.Animator.Play("Burrow", -1, 0f);
            yield return null; // TODO: Can we remove this?
            yield return new WaitForSeconds(
                GearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length -
                                            0.1f);

            // disable collider just before animation finishes
            GearboundSentinel.DisableColliders();
            GearboundSentinel.Shadow.SetActive(false);
            GearboundSentinel.ResetMoveSpeed();
            GearboundSentinel.CurSpeed *= GearboundSentinel.Data.BurrowSpeedMultiplier;
            yield return new WaitForSeconds(0.1f);
            _animating = false;
            GearboundSentinel.StartCoroutine(DiggingSounds());
            GearboundSentinel.Animator.Play("Burrowed", -1, 0f);
        }

        public IEnumerator EndBurrow()
        {
            GearboundSentinel.AudioSource.pitch = 1f;
            if (GearboundSentinel.AudioUnBurrow)
            {
                GearboundSentinel.AudioSource.PlayOneShot(GearboundSentinel.AudioUnBurrow);
            }

            _animating = true;
            GearboundSentinel.Animator.speed = 2f; // play faster until the hit
            GearboundSentinel.CurSpeed = 0f;
            GearboundSentinel.Animator.Play("UnBurrow", -1, 0f);
            const float HitTimeAfterUnburrowStart = 0.1f;
            const float CollisionTimeBeforeUnburrowEnd = 0.4f;

            // wait a short moment before dealing damage
            yield return new WaitForSeconds(HitTimeAfterUnburrowStart);
            GearboundSentinel.Shadow.SetActive(true);
            GearboundSentinel.Animator.speed = 1f;
            var hits = Physics2D
                .OverlapCircleAll(
                    GearboundSentinel.transform.position,
                    1.5f,
                    LayerMask.GetMask("Player")) // todo: config radius
                .Where(x => !x.isTrigger); // the player's feet

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Player>(out var player))
                {
                    player.TakeDamage(GearboundSentinel.Data.BurrowDamage);
                }
            }

            // wait until just before animation end to enable collisions
            yield return new WaitForSeconds(
                GearboundSentinel.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length -
                                            HitTimeAfterUnburrowStart - CollisionTimeBeforeUnburrowEnd);
            GearboundSentinel.EnableColliders();

            // wait the last bit of the animation
            yield return new WaitForSeconds(CollisionTimeBeforeUnburrowEnd);

            // leave Animating true for next time in case OnPhysicsUpdate gets called before OnEnter
            GearboundSentinel.ResetMoveSpeed();

            // in the last phase, GS throws bombs when he pops up
            if (GearboundSentinel.Phase == 2)
            {
                Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Bomb]);
            }

            Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Chase]);
        }

        // quick and dirty
        private IEnumerator DiggingSounds()
        {
            GearboundSentinel.AudioSource.volume = _initialVolume * 0.5f;
            while (!_animating && _isActive)
            {
                GearboundSentinel.AudioSource.clip = GearboundSentinel.AudioBurrowing;
                GearboundSentinel.AudioSource.pitch = Random.Range(0.75f, 0.8f);

                GearboundSentinel.AudioSource.Play();
                GearboundSentinel.AudioSource.time = Random.Range(0.5f, 1.3f);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
