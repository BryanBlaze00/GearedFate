// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// GSBaseState
    /// </summary>
    public abstract class GCBaseState : BaseState<GreatCreator.GreatCreatorState>
    {
        private int _animId;

        private static float _lastSpawnTime;

        private static float _nextSpawnTime;

        private static bool _goingToCenter = false;

        public GreatCreator GreatCreator { get; }

        public bool GoingToCenter => _goingToCenter;

        public GCBaseState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy,
            int animId)
            : base(fsm)
        {
            GreatCreator = enemy;
            _animId = animId;
            SetSpawnTime();
            enemy.OnHitTaken += HandleHitTaken;
        }

        private void HandleHitTaken()
        {
            if (GreatCreator.CurrentHealth == 0)
            {
                fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.Death]);
                return;
            }

            if (GreatCreator.Stage >= 1 && fsm.CurrentState.GetType() != typeof(GCSpinState) && fsm.CurrentState.GetType() != typeof(GCTransformationState))
            {
                fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.Spin]);
            }
        }

        protected IEnumerator GoToCenter()
        {
            _goingToCenter = true;
            var timer = Time.time;
            GreatCreator.Agent.SetDestination(Vector3.zero);
            while (timer + 3 > Time.time || Vector2.Distance(GreatCreator.transform.position, Vector2.zero) > 1)
            {
                yield return null;
            }

            _goingToCenter = false;
        }

        protected void SetAnimationId(int animId)
        {
            _animId = animId;
        }

        protected void SetSpawnTime()
        {
            _lastSpawnTime = Time.time;
            _nextSpawnTime = _lastSpawnTime +
                             Random.Range(GreatCreator.SwarmCoolDown.Min, GreatCreator.SwarmCoolDown.Max);
        }

        protected bool IsReadyToSpawn()
        {
            return Time.time >= _nextSpawnTime;
        }

        protected void PlayAnimation()
        {
            GreatCreator.Animator.Play(_animId);
        }

        public abstract override void OnEnter();

        public abstract override void OnExit();

        public abstract override void OnFrameUpdate();

        public abstract override void OnPhysicsUpdate();
    }
}
