// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

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
            if (player.Knockback.isKnockedback)
            {
                player.RB.linearVelocity = player.Knockback.KnockBackVelocity;
                player.Knockback.isKnockedback = false;
            }
            else
            {
                player.RB.linearVelocity = Vector2.zero;
            }

            startTime = Time.time;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > startTime + data.KnockBackTime)
            {
                player.RB.linearVelocity = Vector2.zero;
            }

            if (Time.time < startTime + data.HitStunTime)
            {
                return;
            }

            fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? player.states[Player.State.Idle] : player.states[Player.State.Move]
            );
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
