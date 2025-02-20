// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerDeadState
    /// </summary>
    public class PlayerDeadState : PlayerBaseState
    {
        private float time;

        public PlayerDeadState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Player.RB.linearVelocity = Vector2.zero;
            Player.RB.Sleep();
            time = Time.time;
        }

        public override void OnExit()
        {
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > time + 3f)
            {
                GameManager.Instance.LoadGameOver();
            }
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
