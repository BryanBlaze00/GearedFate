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

        public PlayerHitState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Player.Knockback.IsKnockedback)
            {
                Player.RB.linearVelocity = Player.Knockback.KnockBackVelocity;
                Player.Knockback.IsKnockedback = false;
            }
            else
            {
                Player.RB.linearVelocity = Vector2.zero;
            }

            startTime = Time.time;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnFrameUpdate()
        {
            if (Time.time > startTime + Data.KnockBackTime)
            {
                Player.RB.linearVelocity = Vector2.zero;
            }

            if (Time.time < startTime + Data.HitStunTime)
            {
                return;
            }

            Fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? Player[Player.State.Idle] : Player[Player.State.Move]);
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }
    }
}
