//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerDeadState
    /// </summary>
    public class PlayerDeadState : PlayerBaseState
    {
        private float time;

        public PlayerDeadState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) : base(
            fsm, player, data, animId)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.player.RB.linearVelocity = Vector2.zero;
            this.player.RB.Sleep();
            this.time = Time.time;
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > this.time + 3f) GameManager.Instance.LoadMainMenu();
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
