using System.Collections.Generic;
using UnityEngine;

namespace Warehouse.Services.Warehouse.Services.Scene.SceneGraph
{
    /// <summary>
    /// Used for SceneGraphController to run new transition from scene From to scene To
    /// </summary>
    public class SceneTransition
    {
        private readonly SceneNames _from;
        private readonly SceneNames _to;
        private readonly List<ISceneTransitionStep> _stepObjects;

        public SceneTransition(SceneNames from, SceneNames to, List<ISceneTransitionStep> stepObjects)
        {
            _from = from;
            _to = to;
            _stepObjects = stepObjects;
        }
        
        public SceneName From => _from;
        public SceneName To => _to;
        public IReadOnlyList<ISceneTransitionStep> StepObjects => _stepObjects;
    }
}