using UnityEngine;
using Warehouse.Services.Camera;
using Warehouse.UI;
using Zenject;

namespace Warehouse.Injection
{
    [DefaultExecutionOrder(1)]
    public class WarehouseInstaller : MonoInstaller
    {
        [SerializeField] private CinemachineCameraMoveNotifier cameraMoveNotifier;
        
        public override void InstallBindings()
        {   
            // Notifiers
            Container
                .Bind<ICameraNotifier>()
                .FromInstance(cameraMoveNotifier)
                .AsSingle();
            
            // UI
            Container
                .Bind<StaminaBarViewModel>()
                .FromNew()
                .AsSingle()
                .WhenInjectedInto<StaminaValueBarView>();
        }
    }   
}