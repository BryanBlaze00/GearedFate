//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using UnityEngine;

namespace BTG
{
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
            int animId) : base(fsm)
        {
            this.GreatCreator = enemy;
            this._animId = animId;
            this.SetSpawnTime();
            enemy.OnHitTaken += this.HandleHitTaken;
        }

        private void HandleHitTaken()
        {
            if (this.GreatCreator.CurrentHealth == 0)
            {
                this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Death]);
                return;
            }

            if (this.GreatCreator.Stage >= 1 && this.fsm.CurrentState.GetType() != typeof(GCSpinState) && this.fsm.CurrentState.GetType() != typeof(GCTransformationState))
            {
                this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Spin]);
            }
        }

        protected IEnumerator GoToCenter()
        {
            _goingToCenter = true;
            var timer = Time.time;
            this.GreatCreator.Agent.SetDestination(Vector3.zero);
            while (timer + 3 > Time.time || Vector2.Distance(this.GreatCreator.transform.position, Vector2.zero) > 1)
            {
                yield return null;
            }

            _goingToCenter = false;
        }

        protected void SetAnimationId(int animId)
        {
            this._animId = animId;
        }

        protected void SetSpawnTime()
        {
            _lastSpawnTime = Time.time;
            _nextSpawnTime = _lastSpawnTime +
                             Random.Range(this.GreatCreator.SwarmCoolDown.Min, this.GreatCreator.SwarmCoolDown.Max);
        }

        protected bool IsReadyToSpawn()
        {
            return Time.time >= _nextSpawnTime;
        }

        protected void PlayAnimation()
        {
            this.GreatCreator.Animator.Play(this._animId);
        }

        public abstract override void OnEnter();

        public abstract override void OnExit();

        public abstract override void OnFrameUpdate();

        public abstract override void OnPhysicsUpdate();
    }
}
