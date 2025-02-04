//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerHeatWave
    /// </summary>
    public class PlayerHeatWave : PlayerBaseState
    {
        public PlayerHeatWave(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) : base(
            fsm, player, data, animId)
        {
            player.AnimEvent.OnSpinChargeUpEvent += this.BlastWave;
            player.AnimEvent.OnSpinFinishedEvent += this.SwitchState;
        }

        ~PlayerHeatWave()
        {
            this.player.AnimEvent.OnSpinChargeUpEvent -= this.BlastWave;
            this.player.AnimEvent.OnSpinFinishedEvent -= this.SwitchState;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(this.player.FlameBeam);
            this.player.RB.linearVelocity = Vector2.zero;
        }

        public override void OnFrameUpdate()
        {
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        public void BlastWave()
        {
            this.player.BlastHeatWave();
        }

        public void SwitchState()
        {
            this.fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? this.player.states[Player.State.Idle] : this.player.states[Player.State.Move]
            );
        }
    }
}
