using System.Collections.Generic;
using UnityEngine;
using Warehouse.Services.Camera;
using Warehouse.Services.Warehouse.Services;
using Warehouse.Services.Warehouse.Services.Scene;
using Zenject;

namespace Warehouse.Injection
{
    [DefaultExecutionOrder(-1)]
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] private CinemachineCameraMoveNotifier cameraMoveNotifier;
        [SerializeField] private PersistenceManager persistenceManager;
        
        public override void InstallBindings()
        {
            Container
                .Bind<PersistenceManager>()
                .FromInstance(persistenceManager)
                .AsTransient();
            
            InstallCameraNotifier();
        }
        
        private void InstallCameraNotifier()
        {
            Container
                .Bind<ICameraNotifier>()
                .FromInstance(cameraMoveNotifier)
                .AsSingle();
        }

        private void Awake()
        {
            persistenceManager.Register(cameraMoveNotifier);
        }
    }   
}