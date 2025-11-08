using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Warehouse.UI
{
    public class StaminaValueBarView : MonoBehaviour
    {
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private Button increaseMaxStaminaButton;
        
        [Inject]
        private StaminaBarViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new();

        private RectTransform _buttonRectTransform;
        private Sequence _buttonClickSequence;  
        
        private void Start()
        {
            _viewModel.Subscribe();
            
            _viewModel.Stamina
                .Subscribe(stamina => staminaText.text = $"Stamina: {(int)stamina}")
                .AddTo(_disposables);

            _viewModel.IncreaseMaxStaminaButtonEnabled
                .Subscribe(buttonEnabled => increaseMaxStaminaButton.enabled = buttonEnabled)
                .AddTo(_disposables);

            _viewModel.IncreaseMaxStaminaCommand.BindTo(increaseMaxStaminaButton)
                .AddTo(_disposables);

            _buttonRectTransform = increaseMaxStaminaButton.GetComponent<RectTransform>();
            increaseMaxStaminaButton.onClick.AddListener(IncreaseMaxStaminaButtonClicked);
        }

        private void IncreaseMaxStaminaButtonClicked()
        {
            _buttonRectTransform.DOShakeAnchorPos(
                0.5f,
                10f,
                20
            );
        }
        
        private void OnDestroy()
        {
            increaseMaxStaminaButton.onClick.RemoveListener(IncreaseMaxStaminaButtonClicked);
            _viewModel.Unsubscribe();
            _disposables.Dispose();
        }
    }
}