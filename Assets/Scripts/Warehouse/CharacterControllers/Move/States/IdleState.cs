using System;
using Warehouse.Models;

namespace Warehouse.CharacterControllers.States
{
    public class IdleState : IMovementState
    {
        public MovementStateType StateType() => MovementStateType.Idle;
        public Action<MovementStateType> RequestStateChange { get; set; }

        public void Tick()
        {
        }

        public void FixedTick()
        {
        }
        
        public void Enter()
        {
        }

        public void Exit()
        {
        }
    }
}
