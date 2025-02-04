//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;
using UnityEngine.Windows;

namespace BTG
{
    /// <summary>
    /// PlayerBaseState
    /// </summary>
    public class PlayerBaseState : BaseState<Player.State>
    {
        protected static PlayerInputHandler Input { get; private set; }
        protected readonly Player player;
        protected readonly PlayerData data;
        private readonly int animId;

        public PlayerBaseState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) :
            base(fsm)
        {
            this.player = player;
            this.data = data;
            this.animId = animId;
            Input = player.Input;
        }

        public override void OnEnter()
        {
            player.Anim.Play(animId, 0, 0);
            OnCheck();
        }

        public override void OnExit()
        {
        } // => player.Anim.SetBool(animId, false);

        public override void OnFrameUpdate()
        {
        }

        protected bool AttackCheck
        {
            get
            {
                if (Input.AttackPressed)
                {
                    fsm.SwitchState(player.states[player.CurrentAbility]);
                    return true;
                }

                return false;
            }
        }

        protected bool DashCheck
        {
            get
            {
                if (Input.DashPressed && ((PlayerDashState)player.states[Player.State.Dash]).CanDash)
                {
                    fsm.SwitchState(player.states[Player.State.Dash]);
                    return true;
                }

                return false;
            }
        }


        /* Previous Controls
        protected bool AbilityChecks
        {
            get
            {
                if (Input.HeatWavePressed)
                {
                    fsm.SwitchState(player.states[Player.State.HeatWave]);
                    return true;
                }
                if (Input.SlashPressed)
                {
                    fsm.SwitchState(player.states[Player.State.Slash]);
                    return true;
                }
                return false;
            }
        }

        protected bool SpecialAbilityChecks
        {
            get
            {
                if (Input.DashPressed && ((PlayerDashState)player.states[Player.State.Dash]).CanDash)
                {
                    fsm.SwitchState(player.states[Player.State.Dash]);
                    return true;
                }
                if (Input.BlazePressed && player.CurrentAttackFuelAmount > 0)
                {
                    fsm.SwitchState(player.states[Player.State.FireBlaze]);
                    return true;
                }
                if (Input.ShootPressed && ((PlayerGearTossState)player.states[Player.State.GearToss]).CanShoot)
                {
                    fsm.SwitchState(player.states[Player.State.GearToss]);
                    return true;
                }
                return false;
            }
        }
        */

        public override void OnPhysicsUpdate()
        {
            OnCheck();
        }

        private void OnCheck()
        {
        }
    }
}
