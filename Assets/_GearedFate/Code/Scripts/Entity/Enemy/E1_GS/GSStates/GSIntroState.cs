// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// GSIntroState
    /// </summary>
    public class GSIntroState : GSBaseState
    {
        public GSIntroState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state, GearboundSentinel gs)
            : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            GearboundSentinel.StartCoroutine(OnEnterCoroutine());
        }

        /// <summary>
        /// A coroutine that waits for the intro animation to finish and then transitions to the Chase state
        /// </summary>
        private IEnumerator OnEnterCoroutine()
        {
            // TODO: Some kind of intro animation?
            yield return new WaitForSeconds(1f);
            Fsm.SwitchState(GearboundSentinel[GearboundSentinel.State.Chase]);
        }
    }
}
