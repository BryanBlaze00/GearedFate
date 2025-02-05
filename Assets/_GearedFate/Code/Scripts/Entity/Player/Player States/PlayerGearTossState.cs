// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerGearTossState
    /// </summary>
    public class PlayerGearTossState : PlayerBaseState
    {
        public PlayerGearTossState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(fsm, player, data, animId)
        {
            LastUsedTime = Time.time - data.GearShootCoolDown;

            player.AnimEvent.OnGearTossEvent += GearToss;
            player.AnimEvent.OnGearTossFinishedEvent += SwitchState;
        }

        ~PlayerGearTossState()
        {
            Player.AnimEvent.OnGearTossEvent -= GearToss;
            Player.AnimEvent.OnGearTossFinishedEvent -= SwitchState;
        }


        public float LastUsedTime { get; private set; }

        public bool CanShoot =>
            Player.CurrentAttackFuelAmount > Data.GearFuelBurnAmount &&
            Time.time > LastUsedTime + Data.GearShootCoolDown;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnFrameUpdate()
        {
            Player.RB.linearVelocity = Data.MoveSpeed * Player.Input.MoveInput;
            Player.SetLookDir();
        }

        public override void OnExit()
        {
            base.OnExit();
            LastUsedTime = Time.time;
        }

        private void GearToss()
        {
            Player.ShootGear();
        }

        private void SwitchState()
        {
            Fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? Player[Player.State.Idle] : Player[Player.State.Move]);
        }
    }
}
