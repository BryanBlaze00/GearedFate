// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerMoveState
    /// </summary>
    public class PlayerMoveState : PlayerBaseState
    {
        public PlayerMoveState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
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

            player.RB.linearVelocity = Input.MoveInput * data.MoveSpeed;
            player.SetLookDir();

            if (Input.MoveInput == Vector2.zero)
            {
                Fsm.SwitchState(player.states[Player.State.Idle]);
            }
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
