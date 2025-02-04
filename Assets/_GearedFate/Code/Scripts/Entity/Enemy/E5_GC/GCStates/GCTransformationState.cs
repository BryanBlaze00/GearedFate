using System.Linq;

namespace BTG
{
    public class GCTransformationState : GCBaseState
    {
        public GCTransformationState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy,
            int animId) : base(fsm, enemy, animId)
        {
            var smb = this.GreatCreator.Animator.GetBehaviours<MultiStepSmb>().First(x => x.Id == "Transform");
            smb.OnStepReached += this.HandleEndTransformation;
        }

        private void HandleEndTransformation(int obj)
        {
            this.GreatCreator.Agent.isStopped = false;
            this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
        }

        public override void OnEnter()
        {
            this.PlayAnimation();
            this.GreatCreator.Agent.isStopped = true;
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
