using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerDashStats", menuName = "Warehouse/Player stats/Dash")]
public class PlayerDashStats : ScriptableObject
{
    [Header("Dash")]
    [SerializeField, InspectorName("Dash speed")] private float baseDashSpeedMult = 2.0f;
    [SerializeField, InspectorName("Dash time")] private float baseDashTime = 0.5f;
    [SerializeField, InspectorName("Dash cooldown")] private float baseDashCooldown = 0.1f;
    [SerializeField, InspectorName("Vertical velocity scale")] private float baseVerticalVelocityDashScale = 0.5f;
    [SerializeField, InspectorName("Is dashing")] private bool baseIsDashing = false;
    [SerializeField, InspectorName("Can dash")] private bool baseCanDash = true;
    
    private ReactiveProperty<float> _dashSpeedMult;
    private ReactiveProperty<float> _dashTime;
    private ReactiveProperty<float> _dashCooldown;
    private ReactiveProperty<float> _verticalVelocityDashScale;
    private ReactiveProperty<bool> _isDashing;
    private ReactiveProperty<bool> _canDash;

    public float DashSpeedMult { get => _dashSpeedMult.Value; set => _dashSpeedMult.Value = value; }
    public float DashTime { get => _dashTime.Value; set => _dashTime.Value = value; }
    public float DashCooldown { get => _dashCooldown.Value; set => _dashCooldown.Value = value; }
    public float VerticalVelocityDashScale { get => _verticalVelocityDashScale.Value; set => _verticalVelocityDashScale.Value = value; }
    public bool IsDashing { get => _isDashing.Value; set => _isDashing.Value = value; }

    public bool CanDash
    {
        get => _canDash.Value;
        set => _canDash.Value = value;
    }

    private void OnEnable()
    {
        // Dash
        _dashSpeedMult = new ReactiveProperty<float>(baseDashSpeedMult);
        _dashTime = new ReactiveProperty<float>(baseDashTime);
        _dashCooldown = new ReactiveProperty<float>(baseDashCooldown);
        _verticalVelocityDashScale = new ReactiveProperty<float>(baseVerticalVelocityDashScale);
        _isDashing = new ReactiveProperty<bool>(baseIsDashing);
        _canDash = new ReactiveProperty<bool>(baseCanDash);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_dashSpeedMult != null) _dashSpeedMult.Value = baseDashSpeedMult;
        if (_dashTime != null) _dashTime.Value = baseDashTime;
        if (_dashCooldown != null) _dashCooldown.Value = baseDashCooldown;
        if (_verticalVelocityDashScale != null) _verticalVelocityDashScale.Value = baseVerticalVelocityDashScale;
        if (_isDashing != null) _isDashing.Value = baseIsDashing;
        if (_canDash != null) _canDash.Value = baseCanDash;
    }
#endif

    private void ResetToDefault()
    {
        DashSpeedMult = baseDashSpeedMult;
        DashTime = baseDashTime;
        DashCooldown = baseDashCooldown;
        VerticalVelocityDashScale = baseVerticalVelocityDashScale;
        IsDashing = baseIsDashing;
        CanDash = baseCanDash;
    }
}
