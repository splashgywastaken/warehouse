using UnityEngine.SceneManagement;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public class SceneTransitionBuilder
    {
        [Inject]
        private FadeInStep.Factory _fadeInFactory;
        [Inject]
        private FadeOutStep.Factory _fadeOutFactory;
        [Inject]
        private LoadSceneStep.Factory _loadSceneFactory;
        [Inject]
        private UnloadSceneStep.Factory _unloadSceneFactory;
        [Inject]
        private BasicLoadingSceneStep.Factory _loadingSceneFactory;

        public FadeInStep CreateFadeInStep(float duration)
        {
            return _fadeInFactory.Create(duration);
        }
        
        public FadeOutStep CreateFadeOutStep(float duration)
        {
            return _fadeOutFactory.Create(duration);
        }

        public LoadSceneStep CreateLoadSceneStep(SceneName sceneName, LoadSceneMode loadMode)
        {
            return _loadSceneFactory.Create(sceneName, loadMode);
        }

        public BasicLoadingSceneStep CreateBasicLoading(SceneName[] sceneNames)
        {
            return _loadingSceneFactory.Create(sceneNames);
        }
        
        public UnloadSceneStep CreateUnloadSceneStep(SceneName sceneName)
        {
            return _unloadSceneFactory.Create(sceneName);
        }
    }
}