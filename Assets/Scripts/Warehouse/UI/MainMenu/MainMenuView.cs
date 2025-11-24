using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Warehouse.UI
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        private Button startButton;
        [SerializeField]
        private Button optionsButton;
        [SerializeField]
        private Button quitButton;
        
        [Inject]
        private MainMenuViewModel _viewModel;
        
        private RectTransform _startButtonRectTransform;
        private RectTransform _optionsButtonRectTransform;
        private RectTransform _quitButtonRectTransform;

        private readonly CompositeDisposable _disposable = new();
        
        private void Start()
        {
            _viewModel.Subscribe();
            
            _startButtonRectTransform = startButton.GetComponent<RectTransform>();
            _optionsButtonRectTransform = optionsButton.GetComponent<RectTransform>();
            _quitButtonRectTransform = quitButton.GetComponent<RectTransform>();
            
            BindButtons();    
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
        
        private void BindButtons()
        {
            _viewModel.StartGameCommand
                .BindTo(startButton)
                .AddTo(_disposable);
            startButton.onClick
                .AddListener(() => ButtonClickShake(_startButtonRectTransform));
            
            _viewModel.OpenOptionsCommand
                .BindTo(optionsButton)
                .AddTo(_disposable);
            optionsButton.onClick
                .AddListener(() => ButtonClickShake(_optionsButtonRectTransform));
            
            _viewModel.QuitCommand
                .BindTo(quitButton)
                .AddTo(_disposable);
            quitButton.onClick
                .AddListener(() => ButtonClickShake(_quitButtonRectTransform));
        }

        private void UnBindButtons()
        {
            _viewModel?.Unsubscribe();
            
            startButton.onClick
                .RemoveListener(() => ButtonClickShake(_startButtonRectTransform));
            
            optionsButton.onClick
                .RemoveListener(() => ButtonClickShake(_optionsButtonRectTransform));
            
            quitButton.onClick
                .RemoveListener(() => ButtonClickShake(_quitButtonRectTransform));
        }
        
        private void ButtonClickShake(RectTransform rc)
        {
            rc.DOShakeAnchorPos(
                0.5f,
                10f,
                20
            );
        }
    }
}
