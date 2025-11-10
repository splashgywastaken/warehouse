using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Warehouse.Character
{
    public class MovementController : IMovementController
    {
        // Camera values
        private Quaternion _cameraRotation;
        private Vector3 _cameraForward;
        private Vector3 _cameraRight;
        
        // Injectables
        // Movement
        private readonly CharacterController _charController;
        private readonly Transform _charTransform;
        // Stats
        private readonly PlayerMovementStats _movementStats;
        private readonly PlayerStaminaStats _staminaStats;
        private readonly PlayerDashStats _dashStats;
        private readonly PlayerJumpStats _jumpStats;
        
        // Events
        /// <summary>
        /// Carries amount of stamina wasted on dash
        /// </summary>
        public event Action<PlayerStaminaStats.StaminaActions> PlayerDashed;
        /// <summary>
        /// Carries amount of stamina wasted on jump
        /// </summary>
        public event Action<PlayerStaminaStats.StaminaActions> PlayerJumped;

        [Inject]
        private MovementController(
            PlayerMovementStats movementStats,
            PlayerStaminaStats staminaStats,
            PlayerDashStats dashStats,
            PlayerJumpStats jumpStats,
            CharacterController charController
        )
        {
            _movementStats = movementStats;
            _staminaStats = staminaStats;
            _dashStats = dashStats;
            _jumpStats = jumpStats;
            _charController = charController;
            _charTransform = _charController.transform;
        }
        
        public void StartJump()
        {
            if (!_charController.isGrounded || _jumpStats.IsJumping) return;
            if (_staminaStats.Stamina < _staminaStats.StaminaJumpCost)
            {
                Debug.Log("Can't jump, out of stamina");
                return;
            }
            PlayerJumped?.Invoke(PlayerStaminaStats.StaminaActions.Jump);
            var value = _movementStats.PlayerVelocity;
            value.y = Mathf.Sqrt(_jumpStats.JumpHeight * -2.0f * _jumpStats.GravityValue);
            _movementStats.PlayerVelocity = value;
            _jumpStats.IsJumping = true;
        }
        
        public void StopJump()
        {
            _jumpStats.IsJumping = false;
        }
        
        public void OnCameraMoved(Vector3 pos)
        {
        }
        
        public void OnCameraRotated(Quaternion rot)
        {
            _cameraRotation = rot;

            var cameraForward = _cameraRotation * Vector3.forward;
            cameraForward.y = 0;
            _cameraForward = cameraForward.normalized;

            var cameraRight = _cameraRotation * Vector3.right;
            cameraRight.y = 0;
            _cameraRight = cameraRight.normalized;
        }
        
        public void Move(Vector2 input)
        {
            if (_dashStats.IsDashing)
            {
                return;
            }
            
            // Make Camera-Relative movement
            var moveDirection = _cameraForward * input.y + _cameraRight * input.x;
            _movementStats.MoveDir = Vector3.ClampMagnitude(moveDirection, 1f);
        }

        private async UniTask DashAsync()
        {
            // Если сделал деш то ставим стейт деша и убираем возможность дешиться
            // пока кулдаун на деш пройдет
            if (_dashStats.IsDashing || !_dashStats.CanDash)
            {
                return;
            }

            if (_staminaStats.Stamina < _staminaStats.StaminaDashCost)
            {
                Debug.Log("Can't dash, out of stamina");
                return;
            }
            PlayerDashed?.Invoke(PlayerStaminaStats.StaminaActions.Dash);
            
            _dashStats.IsDashing = true;
            _dashStats.CanDash = false;
            var vector3 = _movementStats.PlayerVelocity;
            vector3.y = 0;
            _movementStats.PlayerVelocity = vector3;

            var startTime = Time.time;

            // Время за которое происходит деш
            await UniTask.WaitUntil(() =>
            {
                var elapsed = Time.time - startTime;
                var durationReached = elapsed >= _dashStats.DashTime;
                return durationReached;
            });

            _dashStats.IsDashing = false;

            // Кулдаун
            await UniTask.Delay(System.TimeSpan.FromSeconds(_dashStats.DashCooldown));
            _dashStats.CanDash = true;
        }
        
        public void OnDashStarted()
        {
            DashAsync().Forget();
        }
        
        private void UpdateCharacterMovement()
        {
            Vector3 finalMove;
            if (_dashStats.IsDashing)
            {
                finalMove = _movementStats.MoveDir * _dashStats.DashSpeed + _movementStats.PlayerVelocity.y * _dashStats.VerticalVelocityDashScale * Vector3.up;
            }
            else
            {
                finalMove = _movementStats.MoveDir * _movementStats.PlayerSpeed + _movementStats.PlayerVelocity.y * Vector3.up;    
            }
            _charController.Move(finalMove * Time.deltaTime);
        }
        
        private void UpdateCharacterRotation()
        {
            if (_movementStats.MoveDir.sqrMagnitude < 0.01f) {
                return;
            }
            // Целевой поворот персонажа
            var targetRotation = Quaternion.LookRotation(_movementStats.MoveDir, Vector3.up);
            // Плавно вращаем персонажа
            _charTransform.rotation = Quaternion.Slerp(
                _charTransform.rotation,
                targetRotation,
                _movementStats.PlayerRotationSpeed * Time.deltaTime
            );
        }
        
        public void Tick()
        {
            // Apply gravity
            if (!_charController.isGrounded && !_dashStats.IsDashing)
            {
                var vector3 = _movementStats.PlayerVelocity;
                vector3.y += _jumpStats.GravityValue * Time.deltaTime;
                _movementStats.PlayerVelocity = vector3;
            }
        
            // Rotate character
            UpdateCharacterRotation();
            
            // Combine jump + move
            UpdateCharacterMovement();
        }
        
        public void FixedTick()
        {
            if (_charController.isGrounded && _jumpStats.IsJumping)
            {
                PlayerJumped?.Invoke(PlayerStaminaStats.StaminaActions.Jump);
                var vector3 = _movementStats.PlayerVelocity;
                vector3.y = Mathf.Sqrt(_jumpStats.JumpHeight * -2.0f * _jumpStats.GravityValue);
                _movementStats.PlayerVelocity = vector3;
            }
        }
    }
}
