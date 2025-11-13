using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Warehouse.CharacterControllers;
using Warehouse.CharacterControllers.States;
using Warehouse.Models;
using Zenject;

namespace Warehouse.Character
{
    public class MovementController : IMovementController
    {
        private readonly MovementContext _movementContext;
        private readonly MovementStateMachine _fsm;
        
        [Inject]
        private MovementController(
            MovementContext movementContext,
            MovementStateMachine fsm
        )
        {
            _movementContext = movementContext;
            _fsm = fsm;
        }
        
        public void StartJump()
        {
            _fsm.ChangeState(MovementStateType.Jumping);
        }
        
        public void StopJump()
        {
        }
        
        public void OnCameraMoved(Vector3 pos)
        {
        }
        
        public void OnCameraRotated(Quaternion rot)
        {
            _movementContext.CameraRotation = rot;

            var cameraForward = _movementContext.CameraRotation * Vector3.forward;
            cameraForward.y = 0;
            _movementContext.CameraForward = cameraForward.normalized;

            var cameraRight = _movementContext.CameraRotation * Vector3.right;
            cameraRight.y = 0;
            _movementContext.CameraRight = cameraRight.normalized;
        }

        private void UpdateMoveDir(Vector2 input)
        {
            var moveDirection = _movementContext.CameraForward * input.y + _movementContext.CameraRight * input.x;
            _movementContext.MovementStats.MoveDir = Vector3.ClampMagnitude(moveDirection, 1f);   
        }
        
        public void Move(Vector2 input)
        {
            UpdateMoveDir(input);
            if (_fsm.FSMData.CurrentStateType != MovementStateType.Dashing && 
                _fsm.FSMData.CurrentStateType != MovementStateType.Jumping &&
                _fsm.FSMData.CurrentStateType != MovementStateType.Falling)
            {
                _fsm.ChangeState(MovementStateType.Walking);
            }
        }
        
        public void OnDashStarted()
        {
            _fsm.ChangeState(MovementStateType.Dashing);
        }
        
        public void Tick()
        {
            _fsm.Tick();
        }
        
        public void FixedTick()
        {
            // Update all values before updating gravity
            _fsm.FixedTick();
        }
    }
}
