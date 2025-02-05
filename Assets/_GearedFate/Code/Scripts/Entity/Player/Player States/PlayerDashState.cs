// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerDashState
    /// </summary>
    public class PlayerDashState : PlayerBaseState
    {
        private float _startTime;

        public PlayerDashState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
            LastUsedTime = Time.time - data.DashCoolDown;
        }

        public float LastUsedTime { get; private set; }

        public bool CanDash => Time.time > LastUsedTime + Data.DashCoolDown;

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(Player.DashAudio);
            Player.RB.linearVelocity = Player.CurrentDirection * Data.DashForce;
            _startTime = Time.time; // remove later
            Player.IsInvulnerable = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            LastUsedTime = Time.time;
            Player.IsInvulnerable = false;
        }

        public override void OnFrameUpdate()
        {
            if (Time.time >= _startTime + Data.DashTime)
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
                Input.MoveInput == Vector2.zero ? Player[Player.State.Idle] : Player[Player.State.Move]);
        }
    }
}
