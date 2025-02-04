using UnityEngine;

namespace BTG
{
    public class RMStringMazeState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 2f;

        public RMStringMazeState(FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette, int highAnimId, int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            this.Marionette.CircleExpander.SetCircleExpand(true);
            this._enterTime = Time.time;
            this.PlayAnimationHighBodyPart();
            this.PlayAnimationLowBodyPart();
            this.Marionette.OnHitTaken += this.HandleHitTaken;
        }

        private void HandleHitTaken()
        {
            if (Time.time > this._enterTime + this._timeBeforeStateChange) this.fsm.SwitchState(this.Marionette._states[RustedMarionette.RustedMarionetteState.Idle]);
        }

        public override void OnExit()
        {
            this.Marionette.OnHitTaken -= this.HandleHitTaken;
            this.Marionette.CircleExpander.SetCircleExpand(false);
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
