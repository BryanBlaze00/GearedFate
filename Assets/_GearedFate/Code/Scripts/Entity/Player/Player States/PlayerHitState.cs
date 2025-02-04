//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerHitState
    /// </summary>
    public class PlayerHitState : PlayerBaseState
    {
        private float startTime;

        public PlayerHitState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) : base(
            fsm, player, data, animId)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.player.Knockback.isKnockedback)
            {
                this.player.RB.linearVelocity = this.player.Knockback.KnockBackVelocity;
                this.player.Knockback.isKnockedback = false;
            }
            else
            {
                this.player.RB.linearVelocity = Vector2.zero;
            }

            this.startTime = Time.time;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > this.startTime + this.data.KnockBackTime) this.player.RB.linearVelocity = Vector2.zero;

            if (Time.time < this.startTime + this.data.HitStunTime) return;

            this.fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? this.player.states[Player.State.Idle] : this.player.states[Player.State.Move]
            );
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
