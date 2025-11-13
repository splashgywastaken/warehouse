using UnityEngine;
using Zenject;

namespace Warehouse.CharacterControllers
{
    public class MovementContext
    {
        private float _verticalVelocity;
        // OnGround check values
        private readonly LayerMask _groundLayer;
        private readonly float _heightOffset;
        private readonly float _radius;
        
        // Player movement status
        public bool IsGrounded => OnGround();
        // Values on which player's final movement and rotation are based on
        public bool UseGravity { get; set; } = true;
        public Vector3 FinalMove { get; set; }
        public Quaternion FinalRotation { get; set; }
        public float VerticalVelocity
        {
            set
            {
                PreviousVerticalVelocity = _verticalVelocity;
                _verticalVelocity = value;
            }
            get => _verticalVelocity;
        }
        public float PreviousVerticalVelocity { get; set; }

        // Camera values
        public Quaternion CameraRotation;
        public Vector3 CameraForward;
        public Vector3 CameraRight;
        // Character control parameters
        public readonly CharacterController CharController;
        public readonly Transform CharTransform;
        // Player stats
        public readonly PlayerMovementStats MovementStats;
        public readonly PlayerStaminaStats StaminaStats;
        public readonly PlayerDashStats DashStats;
        public readonly PlayerJumpStats JumpStats;
        
        [Inject]
        private MovementContext(
            PlayerMovementStats movementStats,
            PlayerStaminaStats staminaStats,
            PlayerDashStats dashStats,
            PlayerJumpStats jumpStats,
            CharacterController charController
        )
        {
            MovementStats = movementStats;
            StaminaStats = staminaStats;
            DashStats = dashStats;
            JumpStats = jumpStats;
            CharController = charController;
            CharTransform = CharController.transform;
            
            // Everything to work with OnGround() method
            _groundLayer = LayerMask.GetMask("Ground");
            _radius = CharController.radius;
            _heightOffset = _radius / 2;
        }
        
        private bool OnGround()
        {
            return Physics.OverlapSphere(
                CharTransform.position + _heightOffset * Vector3.up,
                _radius,
                _groundLayer
            ).Length != 0;
        }
    }
}
