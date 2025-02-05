namespace BTG
{
    public class CentipedeDeadState : CentipedeBaseState
    {
        private readonly int _animId;

        public CentipedeDeadState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, int animationId, SteamCentipede steamCentipede)
            : base(fsm, steamCentipede)
        {
            _animId = animationId;
        }

        public override void OnEnter()
        {
            Centipede.SetAnimations(_animId, false);
            Centipede.IsAttacking = false;
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
