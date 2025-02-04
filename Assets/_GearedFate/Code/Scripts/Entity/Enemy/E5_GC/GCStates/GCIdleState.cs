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
            this.PlayAnimation();
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            if (this.IsReadyToSpawn()) this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Swarm]);

            if (this.GreatCreator.DistanceToTarget < this.GreatCreator.SafeDistance) this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.RunAway]);
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
