using UnityEngine;
using UnityEngine.SceneManagement;
using Warehouse.Models;
using Warehouse.Services.Warehouse.Services.Scene.SceneGraph;
using Warehouse.UI.Warehouse.UI;
using Zenject;

[DefaultExecutionOrder(0)]
public class SceneGraphInstaller : MonoInstaller
{
    [SerializeField]
    private CanvasGroup fadeGroup;
    [SerializeField]
    private SceneGraphData graphData;
    [SerializeField]
    private LoadingSceneData loadingSceneData;
    
    public override void InstallBindings()
    {
        Container
            .Bind<CanvasGroup>()
            .WithId("fadeGroup")
            .FromInstance(fadeGroup)
            .AsSingle();
        Container
            .Bind<SceneGraphData>()
            .FromInstance(graphData)
            .AsSingle();
        
        Container
            .Bind<ISceneRepository>()
            .To<SceneRepository>()
            .AsSingle();
        Container
            .Bind<SceneGraph>()
            .AsSingle();
        Container
            .Bind<SceneLoader>()
            .AsSingle();
        
        Container.BindFactory<float, FadeInStep, FadeInStep.Factory>().AsSingle();
        Container.BindFactory<float, FadeOutStep, FadeOutStep.Factory>().AsSingle();
        Container.BindFactory<SceneName, LoadSceneMode, LoadSceneStep, LoadSceneStep.Factory>().AsSingle();
        Container.BindFactory<SceneName, UnloadSceneStep, UnloadSceneStep.Factory>().AsSingle();
        BindBasicLoadingScene();
        Container
            .Bind<SceneTransitionBuilder>()
            .AsSingle();
    }

    private void BindBasicLoadingScene()
    {        
        Container
            .Bind<LoadingSceneData>()
            .FromInstance(loadingSceneData)
            .AsSingle();

        Container.BindFactory<SceneName[], BasicLoadingSceneStep, BasicLoadingSceneStep.Factory>().AsSingle();
    }
}