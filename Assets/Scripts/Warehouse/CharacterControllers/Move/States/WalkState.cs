using System;
using UnityEngine;
using Warehouse.Models;
using Zenject;

namespace Warehouse.CharacterControllers.States
{
    public class WalkState : IMovementState
    {
        public MovementStateType StateType() => MovementStateType.Walking;
        public Action<MovementStateType> RequestStateChange { get; set; }

        private readonly MovementContext _movementContext;

        [Inject]
        private WalkState(MovementContext movementContext)
        {
            _movementContext = movementContext;
        }
        
        public void Tick()
        {
        }

        private void UpdateCharacterMovement()
        {
            _movementContext.FinalMove = _movementContext.MovementStats.MoveDir * _movementContext.MovementStats.MoveSpeed;
            _movementContext.CharController.Move(_movementContext.FinalMove * Time.deltaTime);
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
            UpdateCharacterMovement();
            UpdateCharacterRotation();

            if (_movementContext.MovementStats.MoveDir == Vector3.zero)
            {
                RequestStateChange?.Invoke(MovementStateType.Idle);
            }

            if (!_movementContext.IsGrounded)
            {
                RequestStateChange?.Invoke(MovementStateType.Falling);
            }
        }
        
        public void Enter()
        {
        }

        public void Exit()
        {
        }
    }
}
