// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System;

    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    public abstract class BaseState<TState>
        where TState : Enum
    {
        protected BaseState(FiniteStateMachine<TState> fsm)
        {
            Fsm = fsm;
        }

        protected FiniteStateMachine<TState> Fsm { get; private set; }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void OnFrameUpdate();

        public abstract void OnPhysicsUpdate();
    }
}
