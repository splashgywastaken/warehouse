using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public class SceneRepository : ISceneRepository
    {
        private readonly IDictionary<SceneName, UnityEngine.SceneManagement.Scene> _sceneDictionary;

        public SceneRepository()
        {
            _sceneDictionary = new Dictionary<SceneName, UnityEngine.SceneManagement.Scene>();
        }
        
        public bool TryAdd(SceneName sceneName, UnityEngine.SceneManagement.Scene scene)
        {
            if (_sceneDictionary.TryAdd(sceneName, scene)) return true;
            Debug.Log($"Scene {sceneName} already in SceneRepository");
            return false;
        }

        public bool TryGet(SceneName sceneName, out UnityEngine.SceneManagement.Scene scene)
        {
            if (!_sceneDictionary.TryGetValue(sceneName, out scene)) return true;
            Debug.LogWarning($"Scene {sceneName} is not in Scene Repository");
            return false;
        }

        public bool TrySetActive(SceneName sceneName)
        {
            if (!_sceneDictionary.TryGetValue(sceneName, out var value))
            {
                Debug.LogWarning($"Wasn't able to set active {sceneName} scene");
                return false;
            } 
            SceneManager.SetActiveScene(value);
            return true;
        }

        public bool Remove(SceneName sceneName)
        {
            if (_sceneDictionary.Remove(sceneName)) return true;
            Debug.Log($"Wasn't able to remove scene {sceneName} from scene repository");
            return false;
        }
    }
}