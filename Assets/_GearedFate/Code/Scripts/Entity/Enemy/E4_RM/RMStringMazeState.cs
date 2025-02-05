namespace BTG
{
    using UnityEngine;

    public class RMStringMazeState : RMBaseState
    {
        private float _enterTime;

        private float _timeBeforeStateChange = 2f;

        public RMStringMazeState(
            FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette,
            int highAnimId,
            int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            Marionette.CircleExpander.SetCircleExpand(true);
            _enterTime = Time.time;
            PlayAnimationHighBodyPart();
            PlayAnimationLowBodyPart();
            Marionette.OnHitTaken += HandleHitTaken;
        }

        public override void OnExit()
        {
            Marionette.OnHitTaken -= HandleHitTaken;
            Marionette.CircleExpander.SetCircleExpand(false);
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }

        private void HandleHitTaken()
        {
            if (Time.time > _enterTime + _timeBeforeStateChange)
            {
                Fsm.SwitchState(Marionette[RustedMarionette.RustedMarionetteState.Idle]);
            }
        }
    }
}
