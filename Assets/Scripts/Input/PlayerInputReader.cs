using System;
using UnityEngine;

namespace Warehouse.Input {
    public class PlayerInputReader : IInputReader
    {
        private readonly PlayerControls _controls;
        
        public event Action<Vector2> OnMove;
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnAttackPressed;
        public event Action OnAttackReleased;

        public PlayerInputReader()
        {
            _controls = new PlayerControls();
        }
        
        public void Enable()
        {
            _controls.Player.Enable();

            // Move
            _controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            _controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
            // Jump
            _controls.Player.Jump.started += ctx => OnJumpPressed?.Invoke();
            _controls.Player.Jump.canceled += ctx => OnJumpReleased?.Invoke();
            // Attack
            _controls.Player.Attack.started += ctx => OnAttackPressed?.Invoke();
            _controls.Player.Attack.canceled += ctx => OnAttackReleased?.Invoke();
        }
        public void Disable()
        {
            _controls.Player.Disable();
        }
    }
}