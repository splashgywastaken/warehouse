using UnityEngine;
using Warehouse.Models;
using Zenject;
using Warehouse.UI;

namespace Warehouse.Injection
{
    [DefaultExecutionOrder(2)]
    public class PlayerUIInstaller : MonoInstaller<PlayerUIInstaller>
    {
        [Header("Stamina UI")]
        [SerializeField] private GameObject staminaBarUI;
        [SerializeField] private PlayerStaminaStats playerStaminaStats;
        [Header("State machine UI")]
        [SerializeField] private MovementStateMachineData movementStateMachineData;
        
        public override void InstallBindings()
        {
            InstallStateMachineUI();
            InstallStaminaUI();
        }

        private void InstallStateMachineUI()
        {            
            Container
                .Bind<StateMachineViewModel>()
                .FromNew()
                .AsSingle();
            Container
                .Bind<MovementStateMachineData>()
                .FromInstance(movementStateMachineData)
                .AsSingle();
        }
        
        private void InstallStaminaUI()
        {
            Container
                .Bind<StaminaBarViewModel>()
                .FromNew()
                .AsSingle();
            Container
                .Bind<PlayerStaminaStats>()
                .FromInstance(playerStaminaStats)
                .AsSingle();
        }
    }
}