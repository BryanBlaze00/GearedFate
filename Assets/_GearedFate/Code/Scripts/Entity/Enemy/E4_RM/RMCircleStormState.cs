using UnityEngine;

namespace BTG
{
    public class RMCircleStormState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 10f;

        private bool _isAttacking;

        private float _timeBeforeAttack = 1f;

        public RMCircleStormState(FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette, int highAnimId, int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("enter circle storm");
            this._enterTime = Time.time;
            this.PlayAnimationHighBodyPart();
            this.PlayAnimationLowBodyPart();
            this._isAttacking = false;
        }

        public override void OnExit()
        {
            this.Marionette.CircleSpawner.SetSpawningState(false);
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > this._enterTime + this._timeBeforeAttack && !this._isAttacking)
            {
                this._isAttacking = true;
                this.Marionette.CircleSpawner.SetSpawningState(true);
            }

            if (Time.time > this._enterTime + this._timeBeforeStateChange) this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.Idle]);
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
