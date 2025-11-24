namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    public interface ISceneRepository
    {
        public bool TryAdd(SceneName sceneName, UnityEngine.SceneManagement.Scene scene);
        public bool Remove(SceneName sceneName);
        public bool TryGet(SceneName sceneName, out UnityEngine.SceneManagement.Scene scene);
        public bool TrySetActive(SceneName sceneName);
    }
}