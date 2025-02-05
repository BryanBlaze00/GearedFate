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
            player.AnimEvent.OnChargeUpFinishedEvent -= ChargedUp;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(player.FlameBeam);
            chargingUp = true;
            player.RB.linearVelocity = Vector2.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            player.Blaze.GetComponent<Animator>().SetTrigger("BlazeOff");
        }

        public override void OnFrameUpdate()
        {
            if (!Input.AttackPressed || player.CurrentAttackFuelAmount == 0)
            {
                Fsm.SwitchState(player.states[Player.State.Idle]);
                return;
            }

            if (chargingUp)
            {
                return;
            }

            var input = player.Input.MoveInput;
            player.RB.linearVelocity = input * data.FireBlazeMoveSpeed;

            player.SetLookDir();

            var direction = player.CurrentDirection;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            player.ShootPos.rotation = Quaternion.Euler(0, 0, angle - 90);

            var collisions = Physics2D.BoxCastAll(
                player.Blaze.transform.position,
                data.FireBlazeDimension, angle,
                player.CurrentDirection,
                data.FireBlazeDistance,
                data.EnemyLayerMask);
            foreach (var collision in collisions)
            {
                if (collision.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(data.FireBlazeDPS * Time.deltaTime);
                }
            }

            Debug.DrawRay(
                player.Blaze.transform.position,
                player.CurrentDirection * data.FireBlazeDistance); ///visualization for now

            /// I think since this is basically fire, we can keep it at fixed distance despite any enemy falls under it or not.
            /// We don't have to change anything from current code that way

            player.BurnAttackFuel(Time.deltaTime * data.FireBlazeBurnRate);
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        private void ChargedUp()
        {
            player.Blaze.SetActive(true);
            chargingUp = false;
        }
    }
}
