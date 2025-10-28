using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Warehouse.Services.Camera
{
public class CinemachineCameraMoveNotifier : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Минимальное расстояние (в метрах), при котором считается, что камера сдвинулась.")]
    public float positionThreshold = 0.001f;

    [Tooltip("Минимальный угол (в градусах), при котором считается, что камера повернулась.")]
    public float rotationThreshold = 0.05f;

    [Tooltip("Время (сек), через которое считается, что движение прекратилось.")]
    public float stopDelay = 0.2f;

    [Header("События Unity")] 
    public UnityEvent OnMoveStart;
    public UnityEvent OnMove;
    public UnityEvent OnMoveEnd;
    public UnityEvent<Vector3> OnPositionChanged; 
    public UnityEvent<Quaternion> OnRotationChanged;

    public event System.Action MoveStarted;
    public event System.Action MoveEnded;
    public event System.Action<Vector3> PositionChanged;
    public event System.Action<Quaternion> RotationChanged;

    private CinemachineBrain _brain;
    private Transform _cameraTransform;

    private Vector3 _prevPosition;
    private Quaternion _prevRotation;
    private bool _isMoving;
    private float _lastMoveTime;

    private void Awake()
    {
        _brain = FindAnyObjectByType<CinemachineBrain>();
    }

    private void LateUpdate()
    {
        if (!_brain || !_brain.OutputCamera) {
            return;
        }

        if (!_cameraTransform) {
            _cameraTransform = _brain.OutputCamera.transform;
            _prevPosition = _cameraTransform.position;
            _prevRotation = _cameraTransform.rotation;
        }

        var currentPos = _cameraTransform.position;
        var currentRot = _cameraTransform.rotation;

        var posDelta = Vector3.Distance(currentPos, _prevPosition);
        var rotDelta = Quaternion.Angle(currentRot, _prevRotation);

        if (posDelta > positionThreshold || rotDelta > rotationThreshold)
        {
            _lastMoveTime = Time.time;

            if (!_isMoving) {
                _isMoving = true;
                OnMoveStart?.Invoke();
                MoveStarted?.Invoke();
            }

            OnMove?.Invoke();

            if (posDelta > positionThreshold)
            {
                OnPositionChanged?.Invoke(currentPos);
                PositionChanged?.Invoke(currentPos);
            }

            if (rotDelta > rotationThreshold)
            {
                OnRotationChanged?.Invoke(currentRot);
                RotationChanged?.Invoke(currentRot);
            }
        }
        else if (_isMoving && Time.time - _lastMoveTime > stopDelay)
        {
            _isMoving = false;
            OnMoveEnd?.Invoke();
            MoveEnded?.Invoke();
        }

        _prevPosition = currentPos;
        _prevRotation = currentRot;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_cameraTransform == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_cameraTransform.position, 0.05f);
    }
#endif
}

}