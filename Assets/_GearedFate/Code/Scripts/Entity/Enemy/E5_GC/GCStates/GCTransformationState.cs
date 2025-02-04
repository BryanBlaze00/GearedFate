namespace BTG
{
    using System.Linq;

    public class GCTransformationState : GCBaseState
    {
        public GCTransformationState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy,
            int animId) : base(fsm, enemy, animId)
        {
            var smb = GreatCreator.Animator.GetBehaviours<MultiStepSmb>().First(x => x.Id == "Transform");
            smb.OnStepReached += HandleEndTransformation;
        }

        private void HandleEndTransformation(int obj)
        {
            GreatCreator.Agent.isStopped = false;
            fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
        }

        public override void OnEnter()
        {
            PlayAnimation();
            GreatCreator.Agent.isStopped = true;
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
