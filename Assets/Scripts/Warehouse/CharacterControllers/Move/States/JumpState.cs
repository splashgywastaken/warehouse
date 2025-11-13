using System;
using UnityEngine;
using Warehouse.Models;
using Zenject;

namespace Warehouse.CharacterControllers.States
{
    public class JumpState : IMovementState
    {
        public MovementStateType StateType() => MovementStateType.Jumping;
        public Action<MovementStateType> RequestStateChange { get; set; }

        private readonly IStaminaController _staminaController;
        private readonly MovementContext _movementContext;
        private float _previousVerticalVelocity;
        
        [Inject]
        private JumpState(MovementContext movementContext, IStaminaController staminaController)
        {
            _staminaController = staminaController;
            _movementContext = movementContext;
        }
        
        public void Tick()
        {
        }
        
        private float GetVerticalVelocityDelta()
        {
            return _movementContext.VerticalVelocity - _previousVerticalVelocity;
        }
        
        private void UpdateCharacterMovement()
        {
            // Applying gravity to vertical velocity
            _movementContext.VerticalVelocity += _movementContext.JumpStats.GravityValue * Time.deltaTime;
            var finalMove = 
                _movementContext.MovementStats.MoveDir * _movementContext.MovementStats.MoveSpeed +
                _movementContext.VerticalVelocity * Vector3.up;
            _movementContext.CharController.Move(finalMove * Time.deltaTime);
        }
        
        private void UpdateCharacterRotation()
        {
            if (_movementContext.MovementStats.MoveDir.sqrMagnitude < 0.01f) {
                return;
            }
            // Character rotation target
            var targetRotation = Quaternion.LookRotation(_movementContext.MovementStats.MoveDir, Vector3.up);
            // Smoothly rotate character
            _movementContext.CharTransform.rotation = Quaternion.Slerp(
                _movementContext.CharTransform.rotation,
                targetRotation,
                _movementContext.MovementStats.RotationSpeed * Time.deltaTime
            );
        }
        
        public void FixedTick()
        {
            UpdateCharacterMovement();
            UpdateCharacterRotation();
            
            // Check if we need to leave current state
            // Will be true if player starts falling
            var verticalVelocityDelta = GetVerticalVelocityDelta();
            // Get off this state if now character is falling or on ground  
            if (verticalVelocityDelta < 0.0f)
            {
                RequestStateChange?.Invoke(MovementStateType.Falling);
            }
            else if (Mathf.Approximately(verticalVelocityDelta, 0.0f) && _movementContext.IsGrounded)
            {
                RequestStateChange?.Invoke(MovementStateType.Walking);
            }
        }
        
        public void Enter()
        {
            if (!_movementContext.IsGrounded)
            {
                RequestStateChange?.Invoke(MovementStateType.Previous);
                return;
            }
            // stamina check
            if (!_staminaController.ConsumeStamina(PlayerStaminaStats.StaminaActions.Jump))
            {
                RequestStateChange?.Invoke(MovementStateType.Previous);
                return;
            }
            
            if (Mathf.Approximately(_movementContext.VerticalVelocity, 0.0f))
            {
                _movementContext.VerticalVelocity =
                    Mathf.Sqrt(_movementContext.JumpStats.JumpHeight * -2.0f * _movementContext.JumpStats.GravityValue);   
            }
            _movementContext.JumpStats.IsJumping = true;
        }

        public void Exit()
        {
            _movementContext.JumpStats.IsJumping = false;
        }
    }
}
