using System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Warehouse.UI
{
    public class StateMachineView : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentStateText;
        [SerializeField] private TMP_Text previousStateText;
        
        [Inject]
        private StateMachineViewModel _stateMachineViewModel;
        private readonly CompositeDisposable _disposables = new();
        
        private void Start()
        {
            _stateMachineViewModel.CurrentState
                .Subscribe(currentState => currentStateText.text = $"Curr.state: <color=green>{currentState}</color>")
                .AddTo(_disposables);
            _stateMachineViewModel.PreviousState
                .Subscribe(previousState => previousStateText.text = $"Prev.state: <color=red>{previousState}</color>")
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
