// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;

    /// <summary>
    /// PlayerSlashState
    /// </summary>
    public class PlayerSlashState : PlayerBaseState
    {
        private readonly int[] slashIds;
        private float startTime;

        public PlayerSlashState(FiniteStateMachine<Player.State> fsm, Player player, PlayerData data, int animId)
            : base(fsm, player, data, animId)
        {
            slashIds = new int[3];
            slashIds[0] = animId;
            slashIds[1] = Animator.StringToHash("Slash_1");
            slashIds[2] = Animator.StringToHash("Slash_2");
        }

        public override void OnEnter()
        {
            AudioManager.Instance.PlaySFX(player.SlashAudio);
            var id = Random.Range(0, 3);
            player.Anim.Play(slashIds[id]);

            var direction = player.CurrentDirection;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var pos = player.SlashPos;

            pos.parent.rotation = Quaternion.Euler(0, 0, angle + 90);

            var collisions = Physics2D.OverlapCircleAll(pos.position, data.SlashRadius, data.EnemyLayerMask);
            foreach (var collision in collisions)
            {
                collision.GetComponent<IDamagable>()?.TakeDamage(data.SlashDamage);
            }

            startTime = Time.time;
        }

        public override void OnFrameUpdate()
        {
            player.RB.linearVelocity = data.MoveSpeed * player.Input.MoveInput;
            player.SetLookDir();

            if (Time.time > startTime + data.SlashCoolDown)
            {
                fsm.SwitchState(
                    player.Input.MoveInput == Vector2.zero
                        ? player.states[Player.State.Idle]
                        : player.states[Player.State.Move]);
            }
        }
    }
}
