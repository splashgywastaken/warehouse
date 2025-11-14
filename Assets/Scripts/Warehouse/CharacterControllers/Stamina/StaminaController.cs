using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Warehouse.Character.PlayerSignals;
using Zenject;

namespace Warehouse.CharacterControllers
{
    public class StaminaController : IStaminaController
    {
        // Stats 
        private readonly PlayerStaminaStats _staminaStats;
        // Hidden values
        private bool _recharging;
        private CancellationTokenSource _rechargeCts;
        private CancellationTokenSource _rechargeDelayCts;
        private CompositeDisposable _disposables;
        private readonly SignalBus _signalBus;

        [Inject]
        private StaminaController(PlayerStaminaStats staminaStats, SignalBus signalBus)
        {
            _staminaStats = staminaStats;
            _signalBus = signalBus;
            _disposables = new CompositeDisposable();
            _signalBus.Subscribe<PlayerEnabledSignal>(Enable);
            _signalBus.Subscribe<PlayerDisabledSignal>(Disable);
        }
        
        public void Initialize()
        {
        }

        public void Enable()
        {
            _disposables = new CompositeDisposable();
            _staminaStats.maxStaminaRx
                .Where(currentMaxStamina => currentMaxStamina > _staminaStats.Stamina)
                .Subscribe(_ => RestartRecharge())
                .AddTo(_disposables);
        }

        public void Disable()
        {
            _disposables.Dispose();
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<PlayerEnabledSignal>(Enable);
            _signalBus.TryUnsubscribe<PlayerDisabledSignal>(Disable);
            _rechargeCts?.Cancel();
            _rechargeCts?.Dispose();
            _rechargeDelayCts?.Cancel();
            _rechargeDelayCts?.Dispose();
            _disposables.Dispose();
        }
        
        /// <summary>
        /// Returns if stamina can be consumed on specified action
        /// and if it can - runs stamina recharge task
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool ConsumeStamina(PlayerStaminaStats.StaminaActions action)
        {
            if (!_staminaStats.SpendStamina(action))
            {
                return false;
            }
            RestartRecharge();
            return true;
        }

        private void RestartRecharge()
        {
            if (_recharging)
            {
                _rechargeCts?.Cancel();
                _rechargeCts?.Dispose();
                _rechargeDelayCts?.Cancel();
                _rechargeDelayCts?.Dispose();
            }

            _rechargeCts = new CancellationTokenSource();
            _rechargeDelayCts = new CancellationTokenSource();
            RechargeStaminaAsync(_rechargeCts.Token, _rechargeDelayCts.Token).Forget();
        }
        
        private async UniTask RechargeStaminaAsync(CancellationToken rechargeToken, CancellationToken rechargeDelayToken)
        {
            _recharging = true;
            
            try
            {
                // Wait until delay
                await UniTask.Delay(
                    TimeSpan.FromSeconds(_staminaStats.delayBeforeRecharge), 
                    cancellationToken: rechargeDelayToken
                );

                // Recharge stamina
                await UniTask
                    .WaitUntil(
                        () => _staminaStats.RechargeStamina(),
                        PlayerLoopTiming.FixedUpdate,
                        cancellationToken: rechargeToken
                    )
                    .ContinueWith(() =>
                    {
                        _recharging = false;
                    });

            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _rechargeCts?.Dispose();
                _rechargeCts = null;
                _rechargeDelayCts?.Dispose();
                _rechargeDelayCts = null;
            }
        }
    }
}
