using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "PlayerStaminaStats", menuName = "Warehouse/Player stats/Stamina")]
public class PlayerStaminaStats : ScriptableObject
{
    public enum StaminaActions
    {
        Jump = 0,
        Dash = 1
    }
    
    [Header("Stamina")]
    [SerializeField, InspectorName("Stamina")] 
    private float baseStamina = 120.0f;
    [SerializeField, InspectorName("Max stamina")] 
    private float baseMaxStamina = 120.0f;
    [SerializeField, InspectorName("Stamina cap"), Tooltip("Stamina can't be greater than this value")]
    private float baseStaminaCap = 150.0f;
    [SerializeField, InspectorName("Jump cost")] 
    private float baseStaminaJumpCost = 10.0f;
    [SerializeField, InspectorName("Dash cost")]
    private float baseStaminaDashCost = 50.0f;
    [SerializeField, InspectorName("Stamina recharge/s")] 
    private float baseStaminaRecharge = 5.0f;
    [SerializeField] 
    public float delayBeforeRecharge = 0.25f;
    
    [HideInInspector]
    public ReactiveProperty<float> staminaRx;
    [HideInInspector]
    public ReactiveProperty<float> maxStaminaRx;
    [HideInInspector]
    public ReactiveProperty<float> staminaCapRx;
    [HideInInspector]
    public ReactiveProperty<float> staminaJumpCostRx;
    [HideInInspector]
    public ReactiveProperty<float> staminaDashCostRx;
    [HideInInspector]
    public ReactiveProperty<float> staminaRechargeRx;

    public float Stamina { get => staminaRx.Value; set => staminaRx.Value = value; }
    public float MaxStamina { get => maxStaminaRx.Value; set => maxStaminaRx.Value = value; }
    public float StaminaCap { get => staminaCapRx.Value; set => staminaCapRx.Value = value; }
    public float StaminaJumpCost { get => staminaJumpCostRx.Value; set => staminaJumpCostRx.Value = value; }
    public float StaminaDashCost { get => staminaDashCostRx.Value; set => staminaDashCostRx.Value = value; }
    public float StaminaRecharge { get => staminaRechargeRx.Value; set => staminaRechargeRx.Value = value; }

    private void OnEnable()
    {
        // Stamina
        staminaRx = new ReactiveProperty<float>(baseStamina);
        maxStaminaRx = new ReactiveProperty<float>(baseMaxStamina);
        staminaCapRx = new ReactiveProperty<float>(baseStaminaCap);
        staminaJumpCostRx = new ReactiveProperty<float>(baseStaminaJumpCost);
        staminaDashCostRx = new ReactiveProperty<float>(baseStaminaDashCost);
        staminaRechargeRx = new ReactiveProperty<float>(baseStaminaRecharge);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (staminaRx != null) staminaRx.Value = baseStamina;
        if (maxStaminaRx != null) maxStaminaRx.Value = baseMaxStamina;
        if (staminaCapRx != null) staminaCapRx.Value = baseStaminaCap;
        if (staminaJumpCostRx != null) staminaJumpCostRx.Value = baseStaminaJumpCost;
        if (staminaDashCostRx != null) staminaDashCostRx.Value = baseStaminaDashCost;
        if (staminaRechargeRx != null) staminaRechargeRx.Value = baseStaminaRecharge;
    }
#endif

    /// <summary>
    /// Changes stamina after action performed
    /// </summary>
    /// <param name="action">Action on which value is calculated</param>
    /// <returns>If stamina decreased based on action</returns> 
    /// <exception cref="ArgumentOutOfRangeException"> if provided action was not in this enum</exception>
    public bool SpendStamina(StaminaActions action)
    {
        var actionCost = action switch
        {
            StaminaActions.Jump => StaminaJumpCost,
            StaminaActions.Dash => StaminaDashCost,
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
        if (Stamina > actionCost)
        {
            Stamina -= actionCost;
            return true;
        }
        return false;
    }

    public void IncreaseStaminaCap()
    {
        const float increaseRate = 10.0f;
        MaxStamina += increaseRate;
    }

    public bool RechargeStamina()
    {
        Stamina = Mathf.Clamp(Stamina + StaminaRecharge, 0, MaxStamina);
        return Mathf.Approximately(Stamina, MaxStamina);
    }
}
