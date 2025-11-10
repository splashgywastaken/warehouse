using System;
using UnityEngine;
using Zenject;

namespace Warehouse.Character
{
    public interface IMovementController : ITickable, IFixedTickable
    {
        /// <summary>
        /// Carries amount of stamina wasted on dash
        /// </summary>
        public event Action<PlayerStaminaStats.StaminaActions> PlayerDashed;
        /// <summary>
        /// Carries amount of stamina wasted on jump
        /// </summary>
        public event Action<PlayerStaminaStats.StaminaActions> PlayerJumped;

        void StartJump();
        void StopJump();
        void OnCameraMoved(Vector3 pos);
        void OnCameraRotated(Quaternion rot);
        void OnDashStarted();
        void Move(Vector2 input);
    }
}