using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Warehouse.Models;
using Zenject;

namespace Warehouse.CharacterControllers.States
{
    public class DashState : IMovementState
    {
        public MovementStateType StateType() => MovementStateType.Dashing;
        public Action<MovementStateType> RequestStateChange { get; set; }

        private readonly IStaminaController _staminaController;
        private readonly MovementContext _movementContext;
        private readonly SemaphoreSlim _dashLock = new(1, 1);
        private Vector3 _moveDirCached;
        
        [Inject]
        private DashState(MovementContext context, IStaminaController staminaController)
        {
            _movementContext = context;
            _staminaController = staminaController;
        }
        
        public void Tick()
        {
        }
        
        private void UpdateCharacterMovement()
        {
            _movementContext.FinalMove 
                = _moveDirCached *
                  // Dash speed evaluation
                  (_movementContext.DashStats.DashSpeedMult * _movementContext.MovementStats.MoveSpeed);
            _movementContext.CharController.Move(_movementContext.FinalMove * Time.deltaTime);
        }
        
        public void FixedTick()
        {
            if (!_movementContext.DashStats.IsDashing)
            {
                if (_movementContext.IsGrounded)
                {
                    RequestStateChange?.Invoke(MovementStateType.Walking);
                    return;
                }
                RequestStateChange?.Invoke(MovementStateType.Falling);
                return;
            }
            UpdateCharacterMovement();
        }
        
        public void Enter()
        {
            if (_movementContext.MovementStats.MoveDir != Vector3.zero)
            {
                _moveDirCached = _movementContext.MovementStats.MoveDir;
            }

            // Check if player dashed before -> can't dash until cooldown is zeroed
            if (_movementContext.DashStats.IsDashing || !_movementContext.DashStats.CanDash)
            {
                RequestStateChange?.Invoke(MovementStateType.Falling);
                return;
            }
            
            DashAsync().Forget();
        }
        
        private async UniTask DashAsync()
        {
            // If dash was used
            if (!await _dashLock.WaitAsync(0))
            {
                RequestStateChange?.Invoke(MovementStateType.Falling);
                return;
            }

            try
            {
                // Try to consume stamina needed for dash
                // If not enough stamina for dash go to falling state
                if (!_staminaController.ConsumeStamina(PlayerStaminaStats.StaminaActions.Dash))
                {
                    RequestStateChange?.Invoke(MovementStateType.Falling);
                    return;
                }

                // Dash started
                _movementContext.DashStats.IsDashing = true;
                _movementContext.DashStats.CanDash = false;
                _movementContext.UseGravity = false;

                // Adjusting movement context to dash
                _movementContext.VerticalVelocity = 0.0f;

                var startTime = Time.time;

                // Time for dash to complete
                await UniTask.WaitUntil(() =>
                {
                    var elapsed = Time.time - startTime;
                    var durationReached = elapsed >= _movementContext.DashStats.DashTime;
                    return durationReached;
                }).ContinueWith(() =>
                {
                    _movementContext.DashStats.IsDashing = false;
                    _movementContext.UseGravity = true;
                });

                // Dash cooldown
                await UniTask
                    .Delay(TimeSpan.FromSeconds(_movementContext.DashStats.DashCooldown))
                    .ContinueWith(() => { _movementContext.DashStats.CanDash = true; });
            }
            catch (OperationCanceledException)
            {
                
            }
            finally
            {
                _dashLock.Release();
            }
        }

        public void Exit()
        {
            _movementContext.VerticalVelocity = _movementContext.PreviousVerticalVelocity;
        }
    }
}
