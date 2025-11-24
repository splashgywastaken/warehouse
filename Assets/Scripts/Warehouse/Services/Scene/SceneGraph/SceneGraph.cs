using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Controls workflow with scene transitions, used to check if user can make transition between two scenes
    /// </summary>
    public class SceneGraph
    {
        private readonly Dictionary<SceneName, List<SceneTransitionNode>> _edges;

        [Inject]
        public SceneGraph(SceneGraphData data)
        {
            _edges = new Dictionary<SceneName, List<SceneTransitionNode>>();

            foreach (var transition in data.transitions)
            {
                if (!_edges.ContainsKey(transition.From))
                {
                    _edges[transition.From] = new List<SceneTransitionNode>();
                }
                
                _edges[transition.From].Add(transition);
            }
        }

        public bool CanTravel(SceneName from, SceneName to)
        {
            return
                _edges.TryGetValue(from, out var edge) &&
                edge.Any(t => t.To == to);
        }
    }
}