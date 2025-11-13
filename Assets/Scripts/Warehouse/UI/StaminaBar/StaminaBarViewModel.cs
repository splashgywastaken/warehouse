using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace Warehouse.UI
{
    public class StaminaBarViewModel
    {
        private readonly PlayerStaminaStats _staminaStats;

        public ReadOnlyReactiveProperty<float> Stamina { get; private set; }
        public ReadOnlyReactiveProperty<float> MaxStamina { get; private set; }
        public ReactiveProperty<bool> IncreaseMaxStaminaButtonEnabled { get; private set; }
        public ReactiveCommand<Unit> IncreaseMaxStaminaCommand { get; private set; }

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        protected StaminaBarViewModel(PlayerStaminaStats staminaStats)
        {
            _staminaStats = staminaStats;

            // Expose model properties
            Stamina = staminaStats.staminaRx.ToReadOnlyReactiveProperty();
            MaxStamina = staminaStats.maxStaminaRx.ToReadOnlyReactiveProperty();
            IncreaseMaxStaminaButtonEnabled = new ReactiveProperty<bool>(true);

            IncreaseMaxStaminaCommand = staminaStats.maxStaminaRx
                .Select(maxStamina => maxStamina < staminaStats.StaminaCap)
                .ToReactiveCommand();
        }

        public void Subscribe()
        {
            // Handle command execution
            IncreaseMaxStaminaCommand
                .Subscribe(_ => _staminaStats.IncreaseStaminaCap())
                .AddTo(_disposables);
        }
        
        public void Unsubscribe()
        {
            _disposables.Dispose();
        }
    }
}