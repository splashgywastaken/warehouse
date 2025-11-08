using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Warehouse/Player stats/Movement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Movement")]
    [SerializeField, InspectorName("Player speed")] private float basePlayerSpeed = 4.0f;
    [SerializeField, InspectorName("Player rotation speed")] private float basePlayerRotationSpeed = 10.0f;
    [SerializeField, InspectorName("Player velocity")] private Vector3 basePlayerVelocity = Vector3.zero;
    [SerializeField, InspectorName("Move direction")] private Vector3 baseMoveDir = Vector3.zero;
    
    // Private reactive fields
    // Movement
    private ReactiveProperty<float> _playerSpeed;
    private ReactiveProperty<float> _playerRotationSpeed;
    private ReactiveProperty<Vector3> _playerVelocity;
    private ReactiveProperty<Vector3> _moveDir;

    private void OnEnable()
    {
        // Movement
        _playerSpeed = new ReactiveProperty<float>(basePlayerSpeed);
        _playerRotationSpeed = new ReactiveProperty<float>(basePlayerRotationSpeed);
        _playerVelocity = new ReactiveProperty<Vector3>(basePlayerVelocity);
        _moveDir = new ReactiveProperty<Vector3>(baseMoveDir);
    }
    
    // Movement
    public float PlayerSpeed { get => _playerSpeed.Value; set => _playerSpeed.Value = value; }
    public float PlayerRotationSpeed { get => _playerRotationSpeed.Value; set => _playerRotationSpeed.Value = value; }
    public Vector3 PlayerVelocity { get => _playerVelocity.Value; set => _playerVelocity.Value = value; }
    public Vector3 MoveDir { get => _moveDir.Value; set => _moveDir.Value = value; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Держим runtime данные в актуальном состоянии при изменении в инспекторе
        if (_playerSpeed != null) _playerSpeed.Value = basePlayerSpeed;
        if (_playerRotationSpeed != null) _playerRotationSpeed.Value = basePlayerRotationSpeed;
        if (_playerVelocity != null) _playerVelocity.Value = basePlayerVelocity;
        if (_moveDir != null) _moveDir.Value = baseMoveDir;
    }
#endif

    public void ResetToDefaults()
    {
        PlayerSpeed = basePlayerSpeed;
        PlayerRotationSpeed = basePlayerRotationSpeed;
        PlayerVelocity = basePlayerVelocity;
        MoveDir = baseMoveDir;
    }
}
