namespace BTG
{
    using UnityEngine;

    public class RMDeathState : RMBaseState
    {
        public RMDeathState(FiniteStateMachine<RustedMarionette.RustedMarionetteState> fsm,
            RustedMarionette marionette,
            int highAnimId,
            int lowAnimId)
            : base(fsm, marionette, highAnimId, lowAnimId)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("dead");
            PlayAnimationHighBodyPart();
            PlayAnimationLowBodyPart();
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
