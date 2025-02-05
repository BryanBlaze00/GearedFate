namespace BTG
{
    using UnityEngine;

    public class RMIdleState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 3f;

        private bool _lastStateCircleStorm;

        public RMIdleState(
            FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette,
            int highAnimId,
            int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            _enterTime = Time.time;
            PlayAnimationHighBodyPart();
            PlayAnimationLowBodyPart();
            Marionette.AnimatorHighPart.speed = 0f;
            Marionette.AnimatorLowPart.speed = 0f;
            Debug.Log("on enter idle");
        }

        public override void OnExit()
        {
            Marionette.AnimatorHighPart.speed = 1f;
            Marionette.AnimatorLowPart.speed = 1f;
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > _enterTime + _timeBeforeStateChange)
            {
                if (_lastStateCircleStorm)
                {
                    Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.CirclingLines]);
                    _lastStateCircleStorm = false;
                }
                else
                {
                    Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.CircleStorm]);
                    _lastStateCircleStorm = true;
                }
            }
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
