using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Warehouse.Character;
using Warehouse.Services.Camera;
using Zenject;

public class CharacterActionsHandler : MonoBehaviour
{
    [SerializeField] private MovementController movementController;
    [SerializeField] private AttackController attackController;

    [Inject]
    private IInputReader _inputReader;
    private CinemachineCameraMoveNotifier _cameraMoveNotifier;
    private CancellationTokenSource _cts;
    
    private void Awake()
    {
        _cameraMoveNotifier = FindAnyObjectByType<CinemachineCameraMoveNotifier>();
        movementController.CharController = GetComponent<CharacterController>();
        _cts = new CancellationTokenSource();
    }

    private void OnEnable()
    {
        _inputReader.Enable();
        // Player actions
        _inputReader.OnMove += HandleMove;
        _inputReader.OnAttackPressed += HandleAttackStart;
        _inputReader.OnAttackReleased += HandleAttackStop;
        _inputReader.OnJumpPressed += HandleJumpStart;
        _inputReader.OnJumpReleased += HandleJumpStop;

        // Camera notifier related actions
        _cameraMoveNotifier.PositionChanged += HandleCameraPositionChanged;
        _cameraMoveNotifier.RotationChanged += HandleCameraRotationChanged;
        
        ListenCancelInput(_cts.Token).Forget();
    }

    private void OnDisable()
    {
        _inputReader.Disable();
        // Player actions
        _inputReader.OnMove -= HandleMove;
        _inputReader.OnAttackPressed -= HandleAttackStart;
        _inputReader.OnAttackReleased -= HandleAttackStop;
        _inputReader.OnJumpPressed -= HandleJumpStart;
        _inputReader.OnJumpReleased -= HandleJumpStop;

        // Camera notifier related actions
        _cameraMoveNotifier.PositionChanged -= HandleCameraPositionChanged;
        _cameraMoveNotifier.RotationChanged -= HandleCameraRotationChanged;
        
        ListenCancelInput(_cts.Token).Forget();
    }

    private void HandleCameraPositionChanged(Vector3 position)
    {
        movementController.OnCameraMoved(position);
    }
    
    private void HandleCameraRotationChanged(Quaternion rotation)
    {
        movementController.OnCameraRotated(rotation);
    }
    
    private void HandleMove(Vector2 dir)
    {
        movementController.Move(dir);
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
        movementController.StartJump();   
    }

    private void HandleJumpStop()
    {
        movementController.StopJump();
    }

    private async UniTaskVoid ListenCancelInput(CancellationToken token)
    {
        await UniTask.WaitUntil(() => false, cancellationToken: token);
        _inputReader.Disable();
    }
}
