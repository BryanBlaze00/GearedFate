using UnityEngine;

namespace BTG
{
    public class RMSpinningLinesState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 10f;

        private float _timeBeforeAttack = 1f;

        private bool _isAttacking = false;

        public RMSpinningLinesState(FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette, int highAnimId, int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            this._enterTime = Time.time;
            this._isAttacking = false;
            this.PlayAnimationHighBodyPart();
            this.PlayAnimationLowBodyPart();
        }

        public override void OnExit()
        {
            this.Marionette.LineRotater.SetLineState(false);
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > this._enterTime + this._timeBeforeAttack && !this._isAttacking)
            {
                this._isAttacking = true;
                this.Marionette.LineRotater.SetLineState(true);
            }

            if (Time.time > this._enterTime + this._timeBeforeStateChange)
            {
                this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.Idle]);
            }
        }

        public override void OnPhysicsUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
