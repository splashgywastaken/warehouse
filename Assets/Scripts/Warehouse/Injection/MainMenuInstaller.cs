using UnityEngine;
using Warehouse.Services.Warehouse.Services;
using Warehouse.UI;
using Zenject;

[DefaultExecutionOrder(2)]
public class MainMenuInstaller : MonoInstaller<MainMenuInstaller>
{
    public override void InstallBindings()
    {
        Container
            .Bind<MainMenuViewModel>()
            .FromNew()
            .AsSingle();

        Container
            .Bind<PersistenceManager>()
            .FromInstance(PersistenceManager.Instance)
            .AsTransient();
    }
}