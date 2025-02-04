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
            _enterTime = Time.time;
            _isAttacking = false;
            PlayAnimationHighBodyPart();
            PlayAnimationLowBodyPart();
        }

        public override void OnExit()
        {
            Marionette.LineRotater.SetLineState(false);
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > _enterTime + _timeBeforeAttack && !_isAttacking)
            {
                _isAttacking = true;
                Marionette.LineRotater.SetLineState(true);
            }

            if (Time.time > _enterTime + _timeBeforeStateChange)
                fsm.SwitchState(Marionette._states[RustedMarionette.RustedMarionetteState.Idle]);
        }

        public override void OnPhysicsUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}
