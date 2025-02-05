// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerIdleState
    /// </summary>
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Player.RB.linearVelocity = Vector2.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            if (AttackCheck)
            {
                return;
            }

            if (DashCheck)
            {
                return;
            }

            if (Input.MoveInput != Vector2.zero)
            {
                Fsm.SwitchState(Player[Player.State.Move]);
            }
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
