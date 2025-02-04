using UnityEngine;

namespace BTG
{
    public class RMIdleState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 3f;

        private bool _lastStateCircleStorm;

        public RMIdleState(FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm, RustedMarionette marionette,
            int highAnimId, int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            this._enterTime = Time.time;
            this.PlayAnimationHighBodyPart();
            this.PlayAnimationLowBodyPart();
            this.Marionette.AnimatorHighPart.speed = 0f;
            this.Marionette.AnimatorLowPart.speed = 0f;
            Debug.Log("on enter idle");
        }

        public override void OnExit()
        {
            this.Marionette.AnimatorHighPart.speed = 1f;
            this.Marionette.AnimatorLowPart.speed = 1f;
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > this._enterTime + this._timeBeforeStateChange)
            {
                if (this._lastStateCircleStorm)
                {
                    this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.CirclingLines]);
                    this._lastStateCircleStorm = false;
                }
                else
                {
                    this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.CircleStorm]);
                    this._lastStateCircleStorm = true;
                }
            }
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
