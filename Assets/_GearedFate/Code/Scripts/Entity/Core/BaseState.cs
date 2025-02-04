// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System;

    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    public abstract class BaseState<State> where State : Enum
    {
        protected readonly FiniteStateMachine<State> fsm;

        protected BaseState(FiniteStateMachine<State> fsm)
        {
            this.fsm = fsm;
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void OnFrameUpdate();

        public abstract void OnPhysicsUpdate();
    }
}
