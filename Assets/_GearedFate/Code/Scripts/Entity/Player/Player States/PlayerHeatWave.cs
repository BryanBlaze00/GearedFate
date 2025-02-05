// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerHeatWave
    /// </summary>
    public class PlayerHeatWave : PlayerBaseState
    {
        public PlayerHeatWave(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(
            fsm, player, data, animId)
        {
            player.AnimEvent.OnSpinChargeUpEvent += BlastWave;
            player.AnimEvent.OnSpinFinishedEvent += SwitchState;
        }

        ~PlayerHeatWave()
        {
            player.AnimEvent.OnSpinChargeUpEvent -= BlastWave;
            player.AnimEvent.OnSpinFinishedEvent -= SwitchState;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(player.FlameBeam);
            player.RB.linearVelocity = Vector2.zero;
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
            player.BlastHeatWave();
        }

        public void SwitchState()
        {
            Fsm.SwitchState(
                Input.MoveInput == Vector2.zero ? player.states[Player.State.Idle] : player.states[Player.State.Move]);
        }
    }
}
