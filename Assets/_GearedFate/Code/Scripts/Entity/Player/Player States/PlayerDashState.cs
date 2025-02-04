//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerDashState
    /// </summary>
    public class PlayerDashState : PlayerBaseState
    {
        public float LastUsedTime { get; private set; }
        private float startTime;
        public bool CanDash => Time.time > this.LastUsedTime + this.data.DashCoolDown;

        public PlayerDashState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) : base(
            fsm, player, data, animId)
        {
            this.LastUsedTime = Time.time - data.DashCoolDown;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(this.player.DashAudio);
            this.player.RB.linearVelocity = this.player.CurrentDirection * this.data.DashForce;
            this.startTime = Time.time; // remove later
            this.player.isInvulnerable = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            this.LastUsedTime = Time.time;
            this.player.isInvulnerable = false;
        }

        public override void OnFrameUpdate()
        {
            if (Time.time >= this.startTime + this.data.DashTime)
            {
                this.OnDashFinish(); //TODO: Replace with animation finish event
            }
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        public void OnDashFinish()
        {
            this.fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? this.player.states[Player.State.Idle] : this.player.states[Player.State.Move]
            );
        }
    }
}
