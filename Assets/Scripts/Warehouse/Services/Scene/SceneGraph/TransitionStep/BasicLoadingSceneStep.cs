using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Warehouse.Models;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Used instead basic loading scene step
    /// to ensure that player knows how much time he has left for level or scene to load
    /// </summary>
    public class BasicLoadingSceneStep : ISceneTransitionStep
    {
        private readonly SceneName[] _sceneNames;
        private readonly int _sceneNamesLength;
        [Inject]
        private LoadingSceneData _loadingSceneData;

        private BasicLoadingSceneStep(SceneName[] sceneNames)
        {
            _sceneNames = sceneNames;
            _sceneNamesLength = sceneNames.Length;
        }

        public async UniTask Execute(CancellationToken token)
        {
            var loadingScreen = new SceneName(SceneNames.LoadingScene); 
            
            await
                SceneManager
                    .LoadSceneAsync(loadingScreen, LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: token);

            var loadTasks = new UniTask[_sceneNamesLength];
            var progressValues = new float[_sceneNamesLength];
            
            for (var sceneIndex = 0; sceneIndex < _sceneNamesLength; sceneIndex++)
            {
                var index = sceneIndex;
                var loadTask = 
                    SceneManager
                        .LoadSceneAsync(_sceneNames[index], LoadSceneMode.Additive)
                        .ToUniTask(Progress.Create<float>(p =>
                            {
                                progressValues[index] = p;
                                UpdateOverallProgress(progressValues);
                            }
                        ),
                        cancellationToken: token);
                loadTasks[index] = loadTask;
            }

            await UniTask.WhenAll(loadTasks);
            
            await
                SceneManager
                    .UnloadSceneAsync(loadingScreen)
                    .ToUniTask(cancellationToken: token);
        }
        
        private void UpdateOverallProgress(float[] progressValues)
        {
            var totalProgress = progressValues.Sum();
            var overallProgress = totalProgress / progressValues.Length;
            _loadingSceneData.LoadingProgress = overallProgress;
        }
        
        public class Factory : PlaceholderFactory<SceneName[], BasicLoadingSceneStep> { }
    }
}