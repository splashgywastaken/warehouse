using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerJumpStats", menuName = "Warehouse/Player stats/Jump")]
public class PlayerJumpStats : ScriptableObject
{
    [Header("Jump")]
    [SerializeField, InspectorName("Is in air")] private bool baseIsInAir = false;
    [SerializeField, InspectorName("Is jumping")] private bool baseIsJumping = false;
    [SerializeField, InspectorName("Jump height")] private float baseJumpHeight = 5.0f;
    [SerializeField, InspectorName("Gravity value")] private float baseGravityValue = -9.81f;
    
    // Jump
    private ReactiveProperty<bool> _isInAir;
    private ReactiveProperty<bool> _isJumping;
    private ReactiveProperty<float> _jumpHeight;
    private ReactiveProperty<float> _gravityValue;
    
    // Jump
    public bool IsInAir { get => _isInAir.Value; set => _isInAir.Value = value; }
    public bool IsJumping { get => _isJumping.Value; set => _isJumping.Value = value; }
    public float JumpHeight { get => _jumpHeight.Value; set => _jumpHeight.Value = value; }
    public float GravityValue { get => _gravityValue.Value; set => _gravityValue.Value = value; }

    private void OnEnable()
    {
        // Jump
        _isInAir = new ReactiveProperty<bool>(baseIsInAir);
        _isJumping = new ReactiveProperty<bool>(baseIsJumping);
        _jumpHeight = new ReactiveProperty<float>(baseJumpHeight);
        _gravityValue = new ReactiveProperty<float>(baseGravityValue);    
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_isInAir != null) _isInAir.Value = baseIsInAir;
        if (_isJumping != null) _isJumping.Value = baseIsJumping;
        if (_jumpHeight != null) _jumpHeight.Value = baseJumpHeight;
        if (_gravityValue != null) _gravityValue.Value = baseGravityValue;
    }
#endif
    
    public void ResetToDefaults()
    {
        IsInAir = baseIsInAir;
        IsJumping = baseIsJumping;
        JumpHeight = baseJumpHeight;
        GravityValue = baseGravityValue;
    }
}
