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
            this.CurrentState = startState;
            this.CurrentState.OnEnter();
        }

        public void SwitchState(BaseState<State> nextState)
        {
            this.CurrentState.OnExit();
            this.CurrentState = nextState;
            this.CurrentState.OnEnter();
        }
    }
}
