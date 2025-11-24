using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Warehouse.Services.Warehouse.Services;
using Warehouse.Services.Warehouse.Services.Scene;
using Warehouse.Services.Warehouse.Services.Scene.SceneGraph;
using Zenject;

namespace Warehouse.UI
{
    public class MainMenuViewModel
    {
        private const float FadeDuration = 0.5f;
        private readonly SceneGraphController _sceneGraphController;
        private readonly CompositeDisposable _disposable = new();

        #region UICommands
        public ReactiveCommand<Unit> StartGameCommand { get; private set; }
        public ReactiveCommand<Unit> OpenOptionsCommand { get; private set; }
        public ReactiveCommand<Unit> QuitCommand { get; private set; }
        #endregion

        private readonly SceneTransition _switchToGameplay;
        private CancellationToken _token;
        
        [Inject]
        private MainMenuViewModel(
            PersistenceManager persistenceManager
            )
        {
            StartGameCommand = new ReactiveCommand<Unit>();
            OpenOptionsCommand = new ReactiveCommand<Unit>();
            QuitCommand = new ReactiveCommand<Unit>();
            
            try
            {
                _sceneGraphController = persistenceManager.Require<SceneGraphController>();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogWarning(e);
            }
            var transitionBuilder = _sceneGraphController?.GetTransitionBuilder();
            
            _switchToGameplay = new SceneTransition(
                SceneNames.MainMenuUI,
                SceneNames.BasicGameplay,
                new List<ISceneTransitionStep>
                {
                    transitionBuilder?.CreateFadeInStep(FadeDuration),
                    transitionBuilder?.CreateBasicLoading(new SceneName[]{SceneNames.BasicGameplay, SceneNames.PlayerUI}),
                    transitionBuilder?.CreateUnloadSceneStep(SceneNames.MainMenuUI),
                    transitionBuilder?.CreateFadeOutStep(FadeDuration)
                }
            );
        }
        
        public void Subscribe()
        {
            StartGameCommand
                .Subscribe(_ => StartGame())
                .AddTo(_disposable);

            OpenOptionsCommand
                .Subscribe(_ => OpenOptions())
                .AddTo(_disposable);
            
            QuitCommand
                .Subscribe(_ => QuitGame())
                .AddTo(_disposable);
        }
        
        public void Unsubscribe()
        {
            _disposable?.Dispose();
        }
        
        private async void StartGame()
        {
            _token = new CancellationToken();
            await _sceneGraphController.RunTransition(_switchToGameplay, _token);
        }

        private void OpenOptions()
        {
            Debug.Log("open options");
        }

        private void QuitGame()
        {
            Debug.Log("quit game");
        }
    }
}
