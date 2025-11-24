using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Methods to load|unload scenes
    /// </summary>
    public class SceneLoader
    {
        [Inject]
        private readonly ISceneRepository _sceneRepository;
        
        public async UniTask LoadScene(SceneName sceneName, LoadSceneMode loadMode, CancellationToken token)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            await operation.ToUniTask(cancellationToken: token);
            var currentScene = SceneManager.GetActiveScene();
            if (!_sceneRepository.TryAdd(sceneName, currentScene))
            {
                Debug.LogWarning($"Wasn't able to load {sceneName}\ncurrent scene {currentScene.name}");
            }
        }
        
        public async UniTask UnloadScene(SceneName sceneName, CancellationToken token)
        {
            var operation = SceneManager.UnloadSceneAsync(sceneName);
            await operation.ToUniTask(cancellationToken: token);
            var currentScene = SceneManager.GetActiveScene();
            if (!_sceneRepository.Remove(sceneName))
            {
                Debug.LogWarning($"Wasn't able to unload {sceneName}\ncurrent scene {currentScene.name}");   
            }
        }
    }
}