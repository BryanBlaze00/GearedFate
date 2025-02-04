//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// GCIdleState
    /// </summary>
    public class GCSwarmState : GCBaseState
    {
        public GCSwarmState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId) :
            base(fsm, enemy, animId)
        {
            enemy.AnimationEventHandler.OnSpawnFinished += this.HandleSpawnFinished;
        }

        private void HandleSpawnFinished()
        {
            this.GreatCreator.SpawnMinions(Random.Range(this.GreatCreator.SwarmAmount.Min, this.GreatCreator.SwarmAmount.Max));
            this.fsm.SwitchState(this.GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
        }

        public override void OnEnter()
        {
            this.PlayAnimation();
        }

        public override void OnExit()
        {
            this.SetSpawnTime();
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
