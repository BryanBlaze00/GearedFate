//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GCIdleState
    /// </summary>
    public class GCIdleState : GCBaseState
    {
        public GCIdleState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId) :
            base(fsm, enemy, animId)
        {
        }

        public override void OnEnter()
        {
            PlayAnimation();
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            if (IsReadyToSpawn()) fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.Swarm]);

            if (GreatCreator.DistanceToTarget < GreatCreator.SafeDistance)
                fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.RunAway]);
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
