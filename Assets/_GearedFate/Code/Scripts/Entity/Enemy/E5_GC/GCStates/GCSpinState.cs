namespace BTG
{
    using System.Linq;

    public class GCSpinState : GCBaseState
    {
        public GCSpinState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId)
            : base(fsm, enemy, animId)
        {
            var smb = GreatCreator.Animator.GetBehaviours<MultiStepSmb>().First(x => x.Id == "Spin");
            smb.OnStepReached += HandleEndSpin;
        }

        private void HandleEndSpin(int obj)
        {
            fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.RunAway]);
        }

        public override void OnEnter()
        {
            PlayAnimation();
            GreatCreator.StartCoroutine(GreatCreator.ShootProjectiles());
            GreatCreator.Agent.isStopped = true;
        }

        public override void OnExit()
        {
            GreatCreator.Agent.isStopped = false;
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }

        public void SetAnimation(int stringToHash)
        {
            SetAnimationId(stringToHash);
        }
    }
}
