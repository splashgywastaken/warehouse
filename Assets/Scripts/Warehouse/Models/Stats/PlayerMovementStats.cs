using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Warehouse/Player stats/Movement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Movement")]
    [SerializeField, InspectorName("Player speed")] 
    private float baseMoveSpeed = 4.0f;
    [SerializeField, InspectorName("Player rotation speed")] 
    private float baseRotationSpeed = 10.0f;
    [SerializeField, InspectorName("Move direction")] 
    private Vector3 baseMoveDir = Vector3.zero;
    
    // Private reactive fields
    // Movement
    private ReactiveProperty<float> _moveSpeed;
    private ReactiveProperty<float> _rotationSpeed;
    private ReactiveProperty<float> _verticalVelocity;
    private ReactiveProperty<Vector3> _moveDir;

    private void OnEnable()
    {
        // Movement
        _moveSpeed = new ReactiveProperty<float>(baseMoveSpeed);
        _rotationSpeed = new ReactiveProperty<float>(baseRotationSpeed);
        _moveDir = new ReactiveProperty<Vector3>(baseMoveDir);
    }
    
    // Movement
    public float MoveSpeed { get => _moveSpeed.Value; set => _moveSpeed.Value = value; }
    public float RotationSpeed { get => _rotationSpeed.Value; set => _rotationSpeed.Value = value; }
    public Vector3 MoveDir { get => _moveDir.Value; set => _moveDir.Value = value; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Держим runtime данные в актуальном состоянии при изменении в инспекторе
        if (_moveSpeed != null) _moveSpeed.Value = baseMoveSpeed;
        if (_rotationSpeed != null) _rotationSpeed.Value = baseRotationSpeed;
        if (_moveDir != null) _moveDir.Value = baseMoveDir;
    }
#endif

    public void ResetToDefaults()
    {
        MoveSpeed = baseMoveSpeed;
        RotationSpeed = baseRotationSpeed;
        MoveDir = baseMoveDir;
    }
}
