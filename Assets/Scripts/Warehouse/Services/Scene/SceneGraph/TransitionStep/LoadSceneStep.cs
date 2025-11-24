using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public class LoadSceneStep : ScriptableObject, ISceneTransitionStep
    {
        private SceneName _sceneName;
        private LoadSceneMode _loadMode;
        [Inject]
        private SceneLoader _loader;

        [Inject]
        private void Construct(SceneName sceneName, LoadSceneMode loadMode)
        {
            _sceneName = sceneName;
            _loadMode = loadMode;
        }
        
        public async UniTask Execute(CancellationToken token)
        {
            await _loader.LoadScene(_sceneName, _loadMode, token);
        }
        
        public class Factory : PlaceholderFactory<SceneName, LoadSceneMode, LoadSceneStep> {}
    }
}