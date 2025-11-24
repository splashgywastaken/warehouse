using System.Collections.Generic;
using UnityEngine;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Holds data about SceneGraph, user sets manually list of scenes and possible transitions between them
    /// </summary>
    [CreateAssetMenu(menuName = "Warehouse/Scene Graph", fileName = "NewSceneGraphData")]
    public class SceneGraphData : ScriptableObject
    {
        public List<SceneTransitionNode> transitions;
    }
}