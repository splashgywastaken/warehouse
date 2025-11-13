using System;
using UnityEngine;
using Zenject;

namespace Warehouse.Character
{
    public interface IMovementController : ITickable, IFixedTickable
    {
        void StartJump();
        void StopJump();
        void OnCameraMoved(Vector3 pos);
        void OnCameraRotated(Quaternion rot);
        void OnDashStarted();
        void Move(Vector2 input);
    }
}