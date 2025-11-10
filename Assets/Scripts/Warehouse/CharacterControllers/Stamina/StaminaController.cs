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
        private bool _staminaRecharging;
        private CancellationTokenSource _staminaRechargeCts;
        private CompositeDisposable _disposables;
        private readonly SignalBus _signalBus;

        [Inject]
        private StaminaController(PlayerStaminaStats staminaStats, SignalBus signalBus)
        {
            _staminaStats = staminaStats;
            _signalBus = signalBus;
            _disposables = new CompositeDisposable();
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<PlayerEnabledSignal>(OnEnable);
            _signalBus.Subscribe<PlayerDisabledSignal>(OnDisable);
        }

        public void OnEnable()
        {
            _disposables = new CompositeDisposable();
            _staminaStats.maxStaminaRx
                .Where(currentMaxStamina => currentMaxStamina > _staminaStats.Stamina)
                .Subscribe(_ => RechargeStamina())
                .AddTo(_disposables);   
        }

        public void OnDisable()
        {
            _disposables.Dispose();
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<PlayerEnabledSignal>(OnEnable);
            _signalBus.TryUnsubscribe<PlayerDisabledSignal>(OnDisable);
            _staminaRechargeCts?.Cancel();
            _staminaRechargeCts?.Dispose();
            _disposables.Dispose();
        }
        
        public void ConsumeStamina(PlayerStaminaStats.StaminaActions action)
        {
            _staminaStats.SpendStamina(action);
            RechargeStamina();
        }

        private void RechargeStamina()
        {
            if (_staminaRecharging)
            {
                // Debug.Log("<color=orange>[Stamina]</color> Таска уже идёт – отменяем текущую...");
                _staminaRechargeCts?.Cancel();
                _staminaRechargeCts?.Dispose();
                _staminaRechargeCts = null;
            }
            
            UniTask.Void(async () =>
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                _staminaRechargeCts = new CancellationTokenSource();
                // Debug.Log("<color=yellow>[Stamina]</color> Запускаем новую таску восстановления...");
                RechargeStaminaAsync(_staminaRechargeCts.Token).Forget();
            });
            
            _staminaRechargeCts = new CancellationTokenSource();
            RechargeStaminaAsync(_staminaRechargeCts.Token).Forget();            
        }
        
        private async UniTask RechargeStaminaAsync(CancellationToken token)
        {
            _staminaRecharging = true;
            
            try
            {
                // Debug.Log("<color=white>[Stamina]</color> Ожидание перед восстановлением...");
                // Wait until delay
                await UniTask.Delay(TimeSpan.FromSeconds(_staminaStats.delayBeforeRecharge), cancellationToken: token);
                // Debug.Log("<color=green>[Stamina]</color> Начинаем восстанавливать стамину...");

                // Recharge stamina
                while (!_staminaStats.RechargeStamina())
                {
                    // Debug.Log($"<color=cyan>[Stamina]</color> Текущее значение: {_staminaStats.Stamina}");
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                }
                
                // Debug.Log("<color=lime>[Stamina]</color> Стамина полностью восстановлена!");
            }
            catch (OperationCanceledException)
            {
                // Debug.Log("<color=red>[Stamina]</color> Таска отменена.");
            }
            finally
            {
                _staminaRecharging = false;   
                // Debug.Log("<color=white>[Stamina]</color> Процесс восстановления завершён.");
            }
        }
    }
}
