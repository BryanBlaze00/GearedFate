//
// Copyright (c) BTG. All rights reserved.
//

using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerFireBlazeState
    /// </summary>
    public class PlayerFireBlazeState : PlayerBaseState
    {
        private bool chargingUp;
        private IDamagable damagable;

        public PlayerFireBlazeState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) :
            base(fsm, player, data, animId)
        {
            player.AnimEvent.OnChargeUpFinishedEvent += this.ChargedUp;
        }

        ~PlayerFireBlazeState()
        {
            this.player.AnimEvent.OnChargeUpFinishedEvent -= this.ChargedUp;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            AudioManager.Instance.PlaySFX(this.player.FlameBeam);
            this.chargingUp = true;
            this.player.RB.linearVelocity = Vector2.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            this.player.Blaze.GetComponent<Animator>().SetTrigger("BlazeOff");
        }

        public override void OnFrameUpdate()
        {
            if (!Input.AttackPressed || this.player.CurrentAttackFuelAmount == 0)
            {
                this.fsm.SwitchState(this.player.states[Player.State.Idle]);
                return;
            }

            if (this.chargingUp) return;

            var input = this.player.Input.MoveInput;
            this.player.RB.linearVelocity = input * this.data.FireBlazeMoveSpeed;

            this.player.SetLookDir();

            var direction = this.player.CurrentDirection;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            this.player.ShootPos.rotation = Quaternion.Euler(0, 0, angle - 90);

            var collisions = Physics2D.BoxCastAll(
                this.player.Blaze.transform.position,
                this.data.FireBlazeDimension, angle,
                this.player.CurrentDirection,
                this.data.FireBlazeDistance,
                this.data.EnemyLayerMask);
            foreach (var collision in collisions)
                if (collision.collider.TryGetComponent(out IDamagable damagable))
                    damagable.TakeDamage(this.data.FireBlazeDPS * Time.deltaTime);

            Debug.DrawRay(
                this.player.Blaze.transform.position,
                this.player.CurrentDirection * this.data.FireBlazeDistance); ///visualization for now

            ///I think since this is basically fire, we can keep it at fixed distance despite any enemy falls under it or not.
            ///We don't have to change anything from current code that way

            this.player.BurnAttackFuel(Time.deltaTime * this.data.FireBlazeBurnRate);
        }

        public override void OnPhysicsUpdate()
        {
            base.OnPhysicsUpdate();
        }

        private void ChargedUp()
        {
            this.player.Blaze.SetActive(true);
            this.chargingUp = false;
        }
    }
}
