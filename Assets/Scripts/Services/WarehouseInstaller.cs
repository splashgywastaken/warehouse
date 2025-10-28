using System;
using UnityEngine;
using Warehouse.Input;
using Zenject;

namespace Warehouse.Services
{
    public class WarehouseInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInputReader>().To<PlayerInputReader>().AsSingle();
        }
    }   
}