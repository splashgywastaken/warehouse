using System;
using UnityEngine;
using Warehouse.Models;
using Zenject;

namespace Warehouse.CharacterControllers.States
{
    public interface IMovementState : ITickable, IFixedTickable
    {
        MovementStateType StateType();
        Action<MovementStateType> RequestStateChange { get; set; }
        void Enter();
        void Exit();
    }
}
