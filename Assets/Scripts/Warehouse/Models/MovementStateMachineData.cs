using UniRx;

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
    
    public class MovementStateMachineData
    {
        public ReactiveProperty<MovementStateType> CurrentStateRx { get; set; } = new(MovementStateType.Idle);
        public ReactiveProperty<MovementStateType> PreviousStateRx { get; set; } = new(MovementStateType.None);

        public MovementStateType CurrentStateType { get => CurrentStateRx.Value; set => CurrentStateRx.Value = value; } 
        public MovementStateType PreviousStateType { get => PreviousStateRx.Value; set => PreviousStateRx.Value = value; }
    }
}
