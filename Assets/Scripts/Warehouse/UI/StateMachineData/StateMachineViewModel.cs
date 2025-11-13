using UniRx;
using UnityEngine;
using Warehouse.Models;
using Zenject;

namespace Warehouse.UI
{
    public class StateMachineViewModel
    {
        public ReadOnlyReactiveProperty<MovementStateType> CurrentState { get; private set; }
        public ReadOnlyReactiveProperty<MovementStateType> PreviousState { get; private set; }

        private readonly CompositeDisposable _disposables = new();
        
        [Inject]
        private StateMachineViewModel(MovementStateMachineData fsmData)
        {
            CurrentState = fsmData.CurrentStateRx.ToReadOnlyReactiveProperty();
            PreviousState = fsmData.PreviousStateRx.ToReadOnlyReactiveProperty();
        }
    }
}
