using System;
using UnityEngine;
using Warehouse.Models;
using Zenject;

namespace Warehouse.CharacterControllers.States
{
    public class FallState : IMovementState
    {
        public MovementStateType StateType() => MovementStateType.Falling;
        public Action<MovementStateType> RequestStateChange { get; set; }
        
        private readonly MovementContext _movementContext;

        [Inject]
        private FallState(MovementContext movementContext)
        {
            _movementContext = movementContext;
        }
        
        public void Tick()
        {
        }

        private void UpdateCharacterMovement()
        {
            // Applying gravity to vertical velocity
            _movementContext.VerticalVelocity += _movementContext.JumpStats.GravityValue * Time.deltaTime;
            var finalMove = 
                _movementContext.MovementStats.MoveDir * _movementContext.MovementStats.MoveSpeed +
                Vector3.up * (_movementContext.VerticalVelocity * _movementContext.JumpStats.FallMultiplier);
            _movementContext.CharController.Move(finalMove * Time.deltaTime);
        }
        
        private void UpdateCharacterRotation()
        {
            if (_movementContext.MovementStats.MoveDir.sqrMagnitude < 0.01f) {
                return;
            }
            // Целевой поворот персонажа
            var targetRotation = Quaternion.LookRotation(_movementContext.MovementStats.MoveDir, Vector3.up);
            // Плавно вращаем персонажа
            _movementContext.FinalRotation = Quaternion.Slerp(
                _movementContext.CharTransform.rotation,
                targetRotation,
                _movementContext.MovementStats.RotationSpeed * Time.deltaTime
            );
            _movementContext.CharTransform.rotation = _movementContext.FinalRotation;
        }
        
        public void FixedTick()
        {
            if (_movementContext.IsGrounded)
            {
                RequestStateChange?.Invoke(MovementStateType.Walking);
                return;
            }
            
            UpdateCharacterMovement();
            UpdateCharacterRotation();
        }
        
        public void Enter()
        {
            _movementContext.VerticalVelocity = 0.0f;
        }

        public void Exit()
        {
            _movementContext.VerticalVelocity = 0.0f;
        }
    }
}
