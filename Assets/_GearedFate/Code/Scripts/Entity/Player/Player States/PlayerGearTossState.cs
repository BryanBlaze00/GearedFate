// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerGearTossState
    /// </summary>
    public class PlayerGearTossState : PlayerBaseState
    {
        public float LastUsedTime { get; private set; }

        public bool CanShoot =>
            player.CurrentAttackFuelAmount > data.GearFuelBurnAmount &&
                                Time.time > LastUsedTime + data.GearShootCoolDown;

        public PlayerGearTossState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(fsm, player, data, animId)
        {
            LastUsedTime = Time.time - data.GearShootCoolDown;

            player.AnimEvent.OnGearTossEvent += GearToss;
            player.AnimEvent.OnGearTossFinishedEvent += SwitchState;
        }

        ~PlayerGearTossState()
        {
            player.AnimEvent.OnGearTossEvent -= GearToss;
            player.AnimEvent.OnGearTossFinishedEvent -= SwitchState;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnFrameUpdate()
        {
            player.RB.linearVelocity = data.MoveSpeed * player.Input.MoveInput;
            player.SetLookDir();
        }

        public override void OnExit()
        {
            base.OnExit();
            LastUsedTime = Time.time;
        }

        private void GearToss()
        {
            player.ShootGear();
        }

        private void SwitchState()
        {
            fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? player.states[Player.State.Idle] : player.states[Player.State.Move]
            );
        }
    }
}
