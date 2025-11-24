using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Warehouse.Character.PlayerSignals;
using Warehouse.CharacterControllers;
using Warehouse.Services.Camera;
using Warehouse.Services.Warehouse.Services;
using Zenject;

namespace Warehouse.Character
{
    namespace PlayerSignals {
        public class PlayerEnabledSignal { }
        public class PlayerDisabledSignal { }
    }
    
    /// <summary>
    /// Manages all the controllers that controls player actions
    /// </summary>
    public class PlayerKnightManager : MonoBehaviour
    {
        // Controllers
        [Inject]
        private IAttackController _attackController;
        [Inject]
        private IMovementController _movementController;
        // Input
        [Inject]
        private IInputReader _inputReader;
        [Inject]
        private PersistenceManager _persistenceManager;
        // Signals
        [Inject]
        private SignalBus _signalBus;
        
        // Private fields
        // Notifiers
        private ICameraNotifier _cameraMoveNotifier;
        // Misc
        private CancellationTokenSource _cts;
        
        private void Awake()
        {
            _cameraMoveNotifier = _persistenceManager.Get<CinemachineCameraMoveNotifier>(); 
            _cts = new CancellationTokenSource();
        }

        private void Update()
        {
            _movementController.Tick();
            _attackController.Tick();
        }

        private void FixedUpdate()
        {
            _movementController.FixedTick();
            _attackController.FixedTick();
        }
        
        private void OnEnable()
        {
            _signalBus.Fire<PlayerEnabledSignal>();
            
            SubscribeInputReaderEvents();
            SubscribeControllersEvents();
            SubscribeCameraEvents();
            
            ListenCancelInput(_cts.Token).Forget();
        }

        private void SubscribeInputReaderEvents()
        {
            _inputReader.Enable();
            // Player actions
            // Move
            _inputReader.OnMove += HandleMove;
            // Attack
            _inputReader.OnAttackPressed += HandleAttackStart;
            _inputReader.OnAttackReleased += HandleAttackStop;
            // Jump
            _inputReader.OnJumpPressed += HandleJumpStart;
            _inputReader.OnJumpReleased += HandleJumpStop;
            // Dash
            _inputReader.OnDashPressed += HandleDashStart;            
        }
        
        private void SubscribeCameraEvents()
        {
            // Camera notifier related actions
            _cameraMoveNotifier.PositionChanged += HandleCameraPositionChanged;
            _cameraMoveNotifier.RotationChanged += HandleCameraRotationChanged;
        }

        private void SubscribeControllersEvents()
        {
        }
        
        private void UnsubscribeControllersEvents()
        {
        }
        
        private void OnDisable()
        {
            _signalBus.Fire<PlayerDisabledSignal>();
            
            UnsubscribeInputReaderEvents();
            UnsubscribeControllersEvents();
            UnsubscribeCameraEvents();
            
            ListenCancelInput(_cts.Token).Forget();
        }

        private void UnsubscribeInputReaderEvents()
        {
            // Player actions
            // Move
            _inputReader.OnMove -= HandleMove;
            // Attack
            _inputReader.OnAttackPressed -= HandleAttackStart;
            _inputReader.OnAttackReleased -= HandleAttackStop;
            // Jump
            _inputReader.OnJumpPressed -= HandleJumpStart;
            _inputReader.OnJumpReleased -= HandleJumpStop;
            // Dash
            _inputReader.OnDashPressed -= HandleDashStart;            
        }

        private void UnsubscribeCameraEvents()
        {
            // Camera notifier related actions
            _cameraMoveNotifier.PositionChanged -= HandleCameraPositionChanged;
            _cameraMoveNotifier.RotationChanged -= HandleCameraRotationChanged;
        }
        
        private void HandleCameraPositionChanged(Vector3 position)
        {
            _movementController.OnCameraMoved(position);
        }
        
        private void HandleCameraRotationChanged(Quaternion rotation)
        {
            _movementController.OnCameraRotated(rotation);
        }
        
        private void HandleMove(Vector2 dir)
        {
            _movementController.Move(dir);
        }

        private void HandleDashStart()
        {
            _movementController.OnDashStarted();    
        }

        private void HandleAttackStart()
        {
            _attackController.StartAttack();
        }

        private void HandleAttackStop()
        {
            _attackController.StopAttack();
        }

        private void HandleJumpStart()
        {
            _movementController.StartJump();   
        }
        
        private void HandleJumpStop()
        {
            _movementController.StopJump();
        }

        private async UniTaskVoid ListenCancelInput(CancellationToken token)
        {
            await UniTask.WaitUntil(() => false, cancellationToken: token);
            _inputReader.Disable();
        }
    }
}