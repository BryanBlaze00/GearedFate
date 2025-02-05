// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerDashState
    /// </summary>
    public class PlayerDashState : PlayerBaseState
    {

        private float startTime;

        public PlayerDashState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
            LastUsedTime = Time.time - data.DashCoolDown;
        }

        public float LastUsedTime { get; private set; }

        public bool CanDash => Time.time > LastUsedTime + data.DashCoolDown;

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(player.DashAudio);
            player.RB.linearVelocity = player.CurrentDirection * data.DashForce;
            startTime = Time.time; // remove later
            player.isInvulnerable = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            LastUsedTime = Time.time;
            player.isInvulnerable = false;
        }

        public override void OnFrameUpdate()
        {
            if (Time.time >= startTime + data.DashTime)
            {
                OnDashFinish(); // TODO: Replace with animation finish event
            }
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        public void OnDashFinish()
        {
            Fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? player.states[Player.State.Idle] : player.states[Player.State.Move]);
        }
    }
}
