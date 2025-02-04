// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// GCIdleState
    /// </summary>
    public class GCSwarmState : GCBaseState
    {
        public GCSwarmState(FiniteStateMachine<GreatCreator.GreatCreatorState> fsm, GreatCreator enemy, int animId) :
            base(fsm, enemy, animId)
        {
            enemy.AnimationEventHandler.OnSpawnFinished += HandleSpawnFinished;
        }

        private void HandleSpawnFinished()
        {
            GreatCreator.SpawnMinions(Random.Range(GreatCreator.SwarmAmount.Min, GreatCreator.SwarmAmount.Max));
            fsm.SwitchState(GreatCreator.States[GreatCreator.GreatCreatorState.Idle]);
        }

        public override void OnEnter()
        {
            PlayAnimation();
        }

        public override void OnExit()
        {
            SetSpawnTime();
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
