using UnityEngine;
using Warehouse.Models;
using Warehouse.UI.Warehouse.UI;
using Zenject;

public class LoadingScreenUIInstaller : MonoInstaller
{
    [SerializeField]
    private LoadingSceneData loadingSceneData;
    
    public override void InstallBindings()
    {
        Container
            .Bind<LoadingSceneData>()
            .FromInstance(loadingSceneData)
            .AsSingle();
        Container
            .Bind<LoadingScreenViewModel>()
            .AsSingle();
        Container
            .Bind<LoadingScreenView>()
            .AsSingle();
    }
}