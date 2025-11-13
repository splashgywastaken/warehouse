using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerJumpStats", menuName = "Warehouse/Player stats/Jump")]
public class PlayerJumpStats : ScriptableObject
{
    [Header("Jump")]
    [SerializeField, InspectorName("Is jumping")] private bool isJumping = false;
    [SerializeField, InspectorName("Jump height")] private float jumpHeight = 2.5f;
    [SerializeField, InspectorName("Fall multiplier")] private float fallMultiplier = 5.0f; 
    [SerializeField, InspectorName("Gravity value")] private float gravityValue = -9.81f;
    
    // Jump
    private ReactiveProperty<bool> _isInAir;
    private ReactiveProperty<bool> _isJumping;
    
    // Jump
    public bool IsJumping { get => _isJumping.Value; set => _isJumping.Value = value; }
    public float JumpHeight { get => jumpHeight; set => jumpHeight = value; }
    public float GravityValue { get => gravityValue; set => gravityValue = value; }
    public float FallMultiplier { get => fallMultiplier; set => fallMultiplier = value; }

    private void OnEnable()
    {
        // Jump
        _isJumping = new ReactiveProperty<bool>(isJumping);    
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_isJumping != null) _isJumping.Value = isJumping;
    }
#endif
    
    public void ResetToDefaults()
    {
        IsJumping = isJumping;
        JumpHeight = jumpHeight;
        GravityValue = gravityValue;
    }
}
