using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Warehouse.Services.Warehouse.Services.Scene.SceneGraph;
using Zenject;

namespace Warehouse.Services.Warehouse.Services
{
    /// <summary>
    /// Application starting point
    /// </summary>
    public class CoreManager : MonoBehaviour
    {
        [SerializeField]
        private SceneGraphController sceneGraphController;
        [SerializeField]
        private EventSystem eventSystem;
        [SerializeField]
        private float fadeDuration = 0.25f;
        [Inject]
        private PersistenceManager _persistenceManager;
        [Inject]
        private SceneTransitionBuilder _transitionBuilder;

        private SceneTransition _mainMenuTransition;

        private CancellationToken _token;

        private void Awake()
        {
            _mainMenuTransition = new SceneTransition(
                SceneNames.Core,
                SceneNames.MainMenuUI,
                new List<ISceneTransitionStep>
                {
                    _transitionBuilder.CreateFadeInStep(fadeDuration),
                    _transitionBuilder.CreateLoadSceneStep(SceneNames.MainMenuUI, LoadSceneMode.Additive),
                    _transitionBuilder.CreateFadeOutStep(fadeDuration)
                }
            );
            // Persistence manager registration
            _persistenceManager.Register(eventSystem);
            _persistenceManager.Register(this);
            _persistenceManager.Register(sceneGraphController);
        }
        
        private void Start()
        {
            SwitchToMainMenu();
        }

        private async void SwitchToMainMenu()
        {
            await sceneGraphController.RunTransition(_mainMenuTransition, _token);
        }

        private void OnApplicationQuit()
        {
            _persistenceManager.Unregister<EventSystem>();
            _persistenceManager.Unregister<CoreManager>();
            _persistenceManager.Unregister<SceneGraphController>();
        }
    }
}