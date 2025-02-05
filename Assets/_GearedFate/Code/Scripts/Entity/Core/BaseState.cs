// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System;

    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    public abstract class BaseState<State>
        where State : Enum
    {
        protected BaseState(FiniteStateMachine<State> fsm)
        {
            Fsm = fsm;
        }

        protected FiniteStateMachine<State> Fsm { get; private set; }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void OnFrameUpdate();

        public abstract void OnPhysicsUpdate();
    }
}
