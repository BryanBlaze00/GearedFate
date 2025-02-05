// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    /// <summary>
    /// GSBaseState
    /// </summary>
    public class GSBaseState : BaseState<GearboundSentinel.State>
    {
        private GearboundSentinel _gearboundSentinel;
        private GearboundSentinel.State _state;

        public GSBaseState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs)
            : base(fsm)
        {
            _gearboundSentinel = gs;
            _state = state;
        }

        public GearboundSentinel.State State => _state;

        protected GearboundSentinel GearboundSentinel => _gearboundSentinel;

        public override void OnEnter()
        {
            // TODO: Play animations
            _gearboundSentinel.AudioSource.pitch = 1f;
            _gearboundSentinel.EnableColliders(); // workaround rare bug where colliders got stuck disabled
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
