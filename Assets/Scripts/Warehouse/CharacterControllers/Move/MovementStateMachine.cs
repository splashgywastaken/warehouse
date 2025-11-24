using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Warehouse.CharacterControllers.States;
using Warehouse.Models;
using Zenject;

namespace Warehouse.CharacterControllers
{
    public class MovementStateMachine
    {
        private readonly Dictionary<MovementStateType, IMovementState> _states;

        private IMovementState _currentState;

        public event Action<MovementStateType> OnStateChanged;
        public readonly MovementStateMachineData FsmData;

        [Inject]
        public MovementStateMachine(IEnumerable<IMovementState> states, MovementStateMachineData fsmData)
        {
            FsmData = fsmData;
            _states = states.ToDictionary(s => s.StateType());
            foreach (var state in _states.Values)
            {
                state.RequestStateChange = ChangeState;
            }
            ChangeState(MovementStateType.Idle);
        }

        public void ChangeState(MovementStateType newState)
        {
            // If states are equal - return
            if (newState == _currentState?.StateType())
                return;

            switch (newState)
            {
                // If new state is "Previous" then change swap newState and previous
                // case MovementStateType.Previous:
                //     newState = FSMData.PreviousStateType;
                //     (FSMData.CurrentStateType, FSMData.PreviousStateType) = (FSMData.PreviousStateType, FSMData.CurrentStateType);
                //     break;
                // Else set current as previous and current as new state
                default:
                    FsmData.PreviousStateType = FsmData.CurrentStateType;
                    FsmData.CurrentStateType = newState;
                    break;
            }
            _currentState?.Exit();
            _currentState = _states[newState];
            _currentState.Enter();
            
            OnStateChanged?.Invoke(newState);
        }

        public void Tick() => _currentState?.Tick();
        public void FixedTick() => _currentState?.FixedTick();
    }
}
