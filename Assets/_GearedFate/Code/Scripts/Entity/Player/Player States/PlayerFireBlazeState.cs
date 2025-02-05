// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerFireBlazeState
    /// </summary>
    public class PlayerFireBlazeState : PlayerBaseState
    {
        private bool chargingUp;
        private IDamagable damagable;

        public PlayerFireBlazeState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(fsm, player, data, animId)
        {
            player.AnimEvent.OnChargeUpFinishedEvent += ChargedUp;
        }

        ~PlayerFireBlazeState()
        {
            Player.AnimEvent.OnChargeUpFinishedEvent -= ChargedUp;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(Player.FlameBeam);
            chargingUp = true;
            Player.RB.linearVelocity = Vector2.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            Player.Blaze.GetComponent<Animator>().SetTrigger("BlazeOff");
        }

        public override void OnFrameUpdate()
        {
            if (!Input.AttackPressed || Player.CurrentAttackFuelAmount == 0)
            {
                Fsm.SwitchState(Player[Player.State.Idle]);
                return;
            }

            if (chargingUp)
            {
                return;
            }

            var input = Player.Input.MoveInput;
            Player.RB.linearVelocity = input * Data.FireBlazeMoveSpeed;

            Player.SetLookDir();

            var direction = Player.CurrentDirection;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Player.ShootPos.rotation = Quaternion.Euler(0, 0, angle - 90);

            var collisions = Physics2D.BoxCastAll(
                Player.Blaze.transform.position,
                Data.FireBlazeDimension,
                angle,
                Player.CurrentDirection,
                Data.FireBlazeDistance,
                Data.EnemyLayerMask);

            foreach (var collision in collisions)
            {
                if (collision.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(Data.FireBlazeDPS * Time.deltaTime);
                }
            }

            Debug.DrawRay(
                Player.Blaze.transform.position,
                Player.CurrentDirection * Data.FireBlazeDistance); ///visualization for now

            /// I think since this is basically fire, we can keep it at fixed distance despite any enemy falls under it or not.
            /// We don't have to change anything from current code that way
            Player.BurnAttackFuel(Time.deltaTime * Data.FireBlazeBurnRate);
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        private void ChargedUp()
        {
            Player.Blaze.SetActive(true);
            chargingUp = false;
        }
    }
}
