using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Main controller for scene graph API
    /// </summary>
    public class SceneGraphController : MonoBehaviour
    {
        [Inject]
        private readonly SceneGraph _graph;
        [Inject]
        private readonly SceneTransitionBuilder _builder;

        public async UniTask RunTransition(
            SceneTransition transition, 
            CancellationToken token
        )
        {
            if (!_graph.CanTravel(transition.From, transition.To))
            {
                Debug.LogError($"No transition from {transition.From} to {transition.To}");
                return;
            }

            foreach (var step in transition.StepObjects)
            {
                await step.Execute(token);
            }
        }
        public SceneTransitionBuilder GetTransitionBuilder() => _builder;
    }
}