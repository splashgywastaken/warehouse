using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Warehouse.Character
{
    public class MovementController : MonoBehaviour, IMoveController
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private CharacterController charController;
        [SerializeField] private Transform charTransform;
        [Header("Move")]
        [SerializeField] private float playerSpeed = 4.0f;
        [SerializeField] private float playerRotationSpeed = 10.0f;
        [SerializeField] private Vector3 playerVelocity;
        [FormerlySerializedAs("moveInput")] [SerializeField] private Vector3 moveDir;
        [Header("Dash")] 
        [SerializeField] private float dashSpeed = 12.0f;
        [SerializeField] private float dashTime = 0.5f;
        [SerializeField] private float dashCooldown = 0.1f;
        [SerializeField] [Range(0.1f, 2.0f)] private float verticalVelocityDashScale = 0.5f;
        [SerializeField] private bool isDashing;
        [SerializeField] private bool canDash = true;
        [Header("Jump")]
        [SerializeField] private bool isJumping;
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float gravityValue = -9.81f;

        private Quaternion _cameraRotation;
        private Vector3 _cameraForward;
        private Vector3 _cameraRight;
        
        private void Awake()
        {
            playerCamera = Camera.main;
        }

        public CharacterController CharController
        {
            set { 
                charController = value;
                charTransform = charController.transform;
            }
        }


        public void StartJump()
        {
            if (!charController.isGrounded || isJumping) return;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            isJumping = true;
        }

        public void StopJump()
        {
            isJumping = false;
        }
        
        public void OnCameraMoved(Vector3 pos)
        {
        }
        
        public void OnCameraRotated(Quaternion rot)
        {
            _cameraRotation = rot;

            var cameraForward = _cameraRotation * Vector3.forward;
            cameraForward.y = 0;
            _cameraForward = cameraForward.normalized;

            var cameraRight = _cameraRotation * Vector3.right;
            cameraRight.y = 0;
            _cameraRight = cameraRight.normalized;
        }
        
        public void Move(Vector2 input)
        {
            // Make Camera-Relative movement
            var moveDirection = _cameraForward * input.y + _cameraRight * input.x;
            moveDir = Vector3.ClampMagnitude(moveDirection, 1f);
        }

        private async UniTask DashAsync()
        {
            // Если сделал деш то ставим стейт деша и убираем возможность дешится
            // пока кулдаун на деш пройдет
            if (isDashing || !canDash)
            {
                return;
            }
            isDashing = true;
            canDash = false;
            playerVelocity.y = 0;

            var startTime = Time.time;

            // Проведение деша
            await UniTask.WaitUntil(() =>
            {
                var elapsed = Time.time - startTime;
                var durationReached = elapsed >= dashTime;
                return durationReached;
            });

            isDashing = false;

            // Кулдаун
            await UniTask.Delay(System.TimeSpan.FromSeconds(dashCooldown));
            canDash = true;
        }
        
        public void OnDashStarted()
        {
            DashAsync().Forget();
        }
        
        private void UpdateCharacterMovement()
        {
            Vector3 finalMove;
            if (isDashing)
            {
                finalMove = moveDir * dashSpeed + playerVelocity.y * verticalVelocityDashScale * Vector3.up;
            }
            else
            {
                finalMove = moveDir * playerSpeed + playerVelocity.y * Vector3.up;    
            }
            charController.Move(finalMove * Time.deltaTime);
        }
        
        private void UpdateCharacterRotation()
        {
            if (moveDir.sqrMagnitude < 0.01f) {
                return;
            }
            // Целевой поворот персонажа
            var targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            // Плавно вращаем персонажа
            charTransform.rotation = Quaternion.Slerp(
                charTransform.rotation,
                targetRotation,
                playerRotationSpeed * Time.deltaTime
            );
        }
        
        private void Update()
        {
            // Apply gravity
            if (!charController.isGrounded && !isDashing) {
                playerVelocity.y += gravityValue * Time.deltaTime;
            }
        
            // Rotate character
            UpdateCharacterRotation();
            
            // Combine jump + move
            UpdateCharacterMovement();
        }
        
        public void FixedUpdate()
        {
            if (charController.isGrounded && isJumping)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            }
        }
    }
}
