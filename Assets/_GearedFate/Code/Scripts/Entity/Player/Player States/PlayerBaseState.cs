// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    /// <summary>
    /// PlayerBaseState
    /// </summary>
    public class PlayerBaseState : BaseState<Player.State>
    {
        private readonly Player _player;
        private readonly PlayerData _data;
        private readonly int _animId;

        public PlayerBaseState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(fsm)
        {
            _player = player;
            _data = data;
            this._animId = animId;
            Input = player.Input;
        }

        protected static PlayerInputHandler Input { get; private set; }

        protected Player Player => _player;

        protected PlayerData Data => _data;

        protected bool AttackCheck
        {
            get
            {
                if (Input.AttackPressed)
                {
                    Fsm.SwitchState(Player[Player.CurrentAbility]);
                    return true;
                }

                return false;
            }
        }

        protected bool DashCheck
        {
            get
            {
                if (Input.DashPressed && ((PlayerDashState)Player[Player.State.Dash]).CanDash)
                {
                    Fsm.SwitchState(Player[Player.State.Dash]);
                    return true;
                }

                return false;
            }
        }

        public override void OnEnter()
        {
            Player.Anim.Play(_animId, 0, 0);
            OnCheck();
        }

        public override void OnExit()
        {
        } // => player.Anim.SetBool(animId, false);

        public override void OnFrameUpdate()
        {
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
