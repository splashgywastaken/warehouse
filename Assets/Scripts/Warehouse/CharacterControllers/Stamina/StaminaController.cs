using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Warehouse.CharacterControllers
{
    public class StaminaController : MonoBehaviour, IStaminaController
    {
        // Stats
        [SerializeField, Inject] 
        private PlayerStaminaStats staminaStats;
        // Hidden values
        private bool _staminaRecharging;
        private CancellationTokenSource _staminaRechargeCts;
        private CompositeDisposable _disposables;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
        }
        
        private void OnEnable()
        {
            staminaStats.maxStaminaRx
                .Where(currentMaxStamina => currentMaxStamina > staminaStats.Stamina)
                .Subscribe(_ => RechargeStamina())
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }

        private void OnDestroy()
        {
            _staminaRechargeCts?.Cancel();
            _staminaRechargeCts?.Dispose();
            _disposables.Dispose();
        }
        
        public void ConsumeStamina(PlayerStaminaStats.StaminaActions action)
        {
            staminaStats.SpendStamina(action);
            RechargeStamina();
        }

        private void RechargeStamina()
        {
            if (_staminaRecharging)
            {
                Debug.Log("<color=orange>[Stamina]</color> Таска уже идёт – отменяем текущую...");
                _staminaRechargeCts?.Cancel();
                _staminaRechargeCts?.Dispose();
                _staminaRechargeCts = null;
            }
            
            UniTask.Void(async () =>
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

                _staminaRechargeCts = new CancellationTokenSource();
                Debug.Log("<color=yellow>[Stamina]</color> Запускаем новую таску восстановления...");
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
                Debug.Log("<color=white>[Stamina]</color> Ожидание перед восстановлением...");
                // Wait until delay
                await UniTask.Delay(TimeSpan.FromSeconds(staminaStats.delayBeforeRecharge), cancellationToken: token);
                Debug.Log("<color=green>[Stamina]</color> Начинаем восстанавливать стамину...");

                // Recharge stamina
                while (!staminaStats.RechargeStamina())
                {
                    Debug.Log($"<color=cyan>[Stamina]</color> Текущее значение: {staminaStats.Stamina}");
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
                }
                
                Debug.Log("<color=lime>[Stamina]</color> Стамина полностью восстановлена!");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("<color=red>[Stamina]</color> Таска отменена.");
            }
            finally
            {
                _staminaRecharging = false;   
                Debug.Log("<color=white>[Stamina]</color> Процесс восстановления завершён.");
            }
        }
    }
}
