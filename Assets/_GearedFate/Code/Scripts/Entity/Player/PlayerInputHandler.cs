// Copyright (c) BTG. All rights reserved.

namespace BTG
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// PlayerInputHandler
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        public bool AttackPressed { get; private set; }

        public bool DashPressed { get; private set; }
        /* Previous
        public bool BlazePressed { get; private set; }
        public bool HeatWavePressed { get; private set; }
        public bool ShootPressed { get; private set; }
        public bool SlashPressed { get; private set; }
        public Vector2 MousePos { get; private set; }
        public Vector2 LookDir { get; private set; }
        */

        private Player player;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }

        public void OnAbilityChange(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                player.ChangeCurrentAbility((int)ctx.ReadValue<float>());
            }
        }

        public void OnDash(InputAction.CallbackContext ctx) //Might change later to call an event instead of using bool
        {
            if (ctx.performed)
            {
                DashPressed = true;
            }
            else if (ctx.canceled)
            {
                DashPressed = false;
            }
        }

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                AttackPressed = true;
            }
            else if (ctx.canceled)
            {
                AttackPressed = false;
            }
        }

        /* Previous
        public void OnAim(InputAction.CallbackContext ctx)
        {
            MousePos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
            LookDir = (player.Input.MousePos - player.RB.position).normalized;
        }

        public void OnBlaze(InputAction.CallbackContext ctx) //Might change later to call an event instead of using bool
        {
            if (ctx.performed) BlazePressed = true;
            else if (ctx.canceled) BlazePressed = false;
        }

        public void OnSlash(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) SlashPressed = true;
            else if (ctx.canceled) SlashPressed = false;
        }

        public void OnHeatWave(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) HeatWavePressed = true;
            else if (ctx.canceled) HeatWavePressed = false;
        }

        public void OnShoot(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) ShootPressed = true;
            else if (ctx.canceled) ShootPressed = false;
        }*/
    }
}
