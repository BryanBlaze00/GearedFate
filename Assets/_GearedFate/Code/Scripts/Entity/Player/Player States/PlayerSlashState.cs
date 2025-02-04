//
// Copyright (c) BTG. All rights reserved.
//

using System.Data;
using UnityEngine;

namespace BTG
{
    /// <summary>
    /// PlayerSlashState
    /// </summary>
    public class PlayerSlashState : PlayerBaseState
    {
        private readonly int[] slashIds;
        private float startTime;

        public PlayerSlashState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId) :
            base(fsm, player, data, animId)
        {
            this.slashIds = new int[3];
            this.slashIds[0] = animId;
            this.slashIds[1] = Animator.StringToHash("Slash_1");
            this.slashIds[2] = Animator.StringToHash("Slash_2");
        }

        public override void OnEnter()
        {
            AudioManager.Instance.PlaySFX(this.player.SlashAudio);
            var id = Random.Range(0, 3);
            this.player.Anim.Play(this.slashIds[id]);

            var direction = this.player.CurrentDirection;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var pos = this.player.SlashPos;

            pos.parent.rotation = Quaternion.Euler(0, 0, angle + 90);

            var collisions = Physics2D.OverlapCircleAll(pos.position, this.data.SlashRadius, this.data.EnemyLayerMask);
            foreach (var collision in collisions) collision.GetComponent<IDamagable>()?.TakeDamage(this.data.SlashDamage);
            this.startTime = Time.time;
        }

        public override void OnFrameUpdate()
        {
            this.player.RB.linearVelocity = this.data.MoveSpeed * this.player.Input.MoveInput;
            this.player.SetLookDir();

            if (Time.time > this.startTime + this.data.SlashCoolDown)
            {
                this.fsm.SwitchState(
                    this.player.Input.MoveInput == Vector2.zero
                        ? this.player.states[Player.State.Idle]
                        : this.player.states[Player.State.Move]);
            }
        }
    }
}
