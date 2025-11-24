using UnityEngine;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Used for SceneGraphData to set up edges of a graph
    /// </summary>
    [System.Serializable]
    public class SceneTransitionNode
    {
        [SerializeField]
        private SceneNames from;
        [SerializeField]
        private SceneNames to;

        public SceneName From => from;
        public SceneName To => to;
    }
}