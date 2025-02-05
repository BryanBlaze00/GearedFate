// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System;

    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    public class FiniteStateMachine<TState>
        where TState : Enum
    {
        public BaseState<TState> CurrentState { get; private set; }

        public void Initialize(BaseState<TState> startState)
        {
            CurrentState = startState;
            CurrentState.OnEnter();
        }

        public void SwitchState(BaseState<TState> nextState)
        {
            CurrentState.OnExit();
            CurrentState = nextState;
            CurrentState.OnEnter();
        }
    }
}
