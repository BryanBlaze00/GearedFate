//
// Copyright (c) BTG. All rights reserved.
//

using System.Linq;
using UnityEngine;

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
            gearboundSentinel = gs;
            State = state;
        }

        public override void OnEnter()
        {
            //TODO: Play animations
            gearboundSentinel.AudioSource.pitch = 1f;
            gearboundSentinel.EnableColliders(); // workaround rare bug where colliders got stuck disabled
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
