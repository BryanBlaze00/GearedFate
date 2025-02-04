//
// Copyright (c) BTG. All rights reserved.
//

namespace BTG
{
    /// <summary>
    /// GSBaseState
    /// </summary>
    public class GSBaseState : BaseState<GearboundSentinel.State>
    {
        protected GearboundSentinel gearboundSentinel;
        public GearboundSentinel.State State;

        public GSBaseState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm)
        {
            this.gearboundSentinel = gs;
            this.State = state;
        }

        public override void OnEnter()
        {
            //TODO: Play animations
            this.gearboundSentinel.AudioSource.pitch = 1f;
            this.gearboundSentinel.EnableColliders(); // workaround rare bug where colliders got stuck disabled
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
