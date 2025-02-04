//
// Copyright (c) BTG. All rights reserved.
//

using System;

namespace BTG
{
    /// <summary>
    /// Base Super Class for entity states
    /// </summary>
    public class FiniteStateMachine<State> where State : Enum
    {
        public BaseState<State> CurrentState { get; private set; }

        public void Initialize(BaseState<State> startState)
        {
            CurrentState = startState;
            CurrentState.OnEnter();
        }

        public void SwitchState(BaseState<State> nextState)
        {
            CurrentState.OnExit();
            CurrentState = nextState;
            CurrentState.OnEnter();
        }
    }
}
