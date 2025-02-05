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
        private Player _player;

        public Vector2 MoveInput { get; private set; }

        public bool AttackPressed { get; private set; }

        public bool DashPressed { get; private set; }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }

        public void OnAbilityChange(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                _player.ChangeCurrentAbility((int)ctx.ReadValue<float>());
            }
        }

        // Might change later to call an event instead of using bool
        public void OnDash(InputAction.CallbackContext ctx)
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

        private void Awake()
        {
            _player = GetComponent<Player>();
        }
    }
}
