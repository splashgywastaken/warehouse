using UniRx;
using UnityEngine;

namespace Warehouse.Models
{
    public enum MovementStateType
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        Previous,
        None
    }
    
    [CreateAssetMenu(menuName = "Warehouse/FSM data/Movement state machine data", fileName = "NewMovementStateMachineData")]
    public class MovementStateMachineData : ScriptableObject
    {
        public ReactiveProperty<MovementStateType> CurrentStateRx { get; set; } = new(MovementStateType.Idle);
        public ReactiveProperty<MovementStateType> PreviousStateRx { get; set; } = new(MovementStateType.None);

        public MovementStateType CurrentStateType { get => CurrentStateRx.Value; set => CurrentStateRx.Value = value; } 
        public MovementStateType PreviousStateType { get => PreviousStateRx.Value; set => PreviousStateRx.Value = value; }
    }
}
