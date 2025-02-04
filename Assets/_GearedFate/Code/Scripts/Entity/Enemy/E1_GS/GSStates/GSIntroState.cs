//
// Copyright (c) BTG. All rights reserved.
//

using System.Collections;
using System.Linq;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GSIntroState
    /// </summary>
    public class GSIntroState : GSBaseState
    {
        public GSIntroState(FiniteStateMachine<GearboundSentinel.State> fsm, GearboundSentinel.State state,
            GearboundSentinel gs) : base(fsm, state, gs)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            gearboundSentinel.StartCoroutine(OnEnterCoroutine());
        }

        /// <summary>
        /// A coroutine that waits for the intro animation to finish and then transitions to the Chase state
        /// </summary>
        private IEnumerator OnEnterCoroutine()
        {
            // TODO: Some kind of intro animation?
            yield return new WaitForSeconds(1f);
            fsm.SwitchState(gearboundSentinel.States[GearboundSentinel.State.Chase]);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            base.OnFrameUpdate();
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
