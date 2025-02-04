//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerMoveState
    /// </summary>
    public class PlayerMoveState : PlayerBaseState
    {
        public PlayerMoveState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) : base(
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
            if (this.AttackCheck)
            {
                return;
            }

            if (this.DashCheck)
            {
                return;
            }

            this.player.RB.linearVelocity = Input.MoveInput * this.data.MoveSpeed;
            this.player.SetLookDir();

            if (Input.MoveInput == Vector2.zero)
            {
                this.fsm.SwitchState(this.player.states[Player.State.Idle]);
            }
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
