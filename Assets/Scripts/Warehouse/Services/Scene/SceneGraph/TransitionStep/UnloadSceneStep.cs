using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public class UnloadSceneStep : ISceneTransitionStep
    {
        private SceneName _sceneName;

        [Inject]
        private SceneLoader _loader;

        [Inject]
        private void Construct(SceneName sceneName)
        {
            _sceneName = sceneName;
        }

        public async UniTask Execute(CancellationToken token)
        {
            await _loader.UnloadScene(_sceneName, token);
        }
        
        public class Factory : PlaceholderFactory<SceneName, UnloadSceneStep> {}
    }
}