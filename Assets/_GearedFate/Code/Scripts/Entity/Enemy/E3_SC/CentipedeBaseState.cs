namespace BTG
{
    public abstract class CentipedeBaseState : BaseState<SteamCentipede.CentipedeState>
    {
        protected SteamCentipede Centipede { get; }

        protected CentipedeBaseState(FiniteStateMachine<SteamCentipede.CentipedeState> fsm, SteamCentipede centipede) :
            base(fsm)
        {
            this.Centipede = centipede;
        }

        public abstract override void OnEnter();


        public abstract override void OnExit();


        public abstract override void OnFrameUpdate();


        public abstract override void OnPhysicsUpdate();
    }
}
