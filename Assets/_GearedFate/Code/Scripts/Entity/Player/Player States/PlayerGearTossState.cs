//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerGearTossState
    /// </summary>
    public class PlayerGearTossState : PlayerBaseState
    {
        public float LastUsedTime { get; private set; }

        public bool CanShoot =>
            this.player.CurrentAttackFuelAmount > this.data.GearFuelBurnAmount &&
                                Time.time > this.LastUsedTime + this.data.GearShootCoolDown;

        public PlayerGearTossState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) :
            base(fsm, player, data, animId)
        {
            this.LastUsedTime = Time.time - data.GearShootCoolDown;

            player.AnimEvent.OnGearTossEvent += this.GearToss;
            player.AnimEvent.OnGearTossFinishedEvent += this.SwitchState;
        }

        ~PlayerGearTossState()
        {
            this.player.AnimEvent.OnGearTossEvent -= this.GearToss;
            this.player.AnimEvent.OnGearTossFinishedEvent -= this.SwitchState;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnFrameUpdate()
        {
            this.player.RB.linearVelocity = this.data.MoveSpeed * this.player.Input.MoveInput;
            this.player.SetLookDir();
        }

        public override void OnExit()
        {
            base.OnExit();
            this.LastUsedTime = Time.time;
        }

        private void GearToss()
        {
            this.player.ShootGear();
        }

        private void SwitchState()
        {
            this.fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? this.player.states[Player.State.Idle] : this.player.states[Player.State.Move]
            );
        }
    }
}
