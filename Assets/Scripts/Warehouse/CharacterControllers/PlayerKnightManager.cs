using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Warehouse.CharacterControllers;
using Warehouse.Services.Camera;
using Zenject;

namespace Warehouse.Character
{
    /// <summary>
    /// Manages all the controllers that controls player actions
    /// </summary>
    public class PlayerKnightManager : MonoBehaviour
    {
        // Controllers
        [SerializeField] private AttackController attackController;
        [SerializeField] private StaminaController staminaController; 
        
        // Injectables
        // Controllers
        [Inject]
        private IMoveController _movementController;
        // Input
        [Inject]
        private IInputReader _inputReader;
        
        // Notifiers
        [Inject]
        private ICameraNotifier _cameraMoveNotifier;
        
        // Private fields
        // Misc
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cameraMoveNotifier = FindAnyObjectByType<CinemachineCameraMoveNotifier>();
            _cts = new CancellationTokenSource();
        }

        private void Update()
        {
            _movementController.Tick();
        }

        private void FixedUpdate()
        {
            _movementController.TickFixed();
        }
        
        private void OnEnable()
        {
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
            _movementController.PlayerJumped += HandlePlayerJumped;
            _movementController.PlayerDashed += HandlePlayerDashed;
        }
        
        private void UnsubscribeControllersEvents()
        {
            _movementController.PlayerJumped -= HandlePlayerJumped;
            _movementController.PlayerDashed -= HandlePlayerDashed;
        }
        
        private void OnDisable()
        {
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

        private void HandlePlayerDashed(PlayerStaminaStats.StaminaActions action)
        {
            staminaController.ConsumeStamina(action);
        }
        
        private void HandleAttackStart()
        {
            attackController.StartAttack();
        }

        private void HandleAttackStop()
        {
            attackController.StopAttack();
        }

        private void HandleJumpStart()
        {
            _movementController.StartJump();   
        }

        private void HandlePlayerJumped(PlayerStaminaStats.StaminaActions action)
        {
            staminaController.ConsumeStamina(action);
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