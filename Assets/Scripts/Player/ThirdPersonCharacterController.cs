using ParkourSystem.CameraSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ParkourSystem.CharacterControlSystem
{
    public class ThirdPersonCharacterController : MonoBehaviour
    {
        private CharacterController characterController;
        private CameraController cameraController;
        [Header("Movement Settings")]
        [SerializeField] Animator animator;
        [SerializeField] float movementSpeed;
        [SerializeField] float rotationSpeed = 500f;
        [SerializeField] float movementDampTime = 0.2f;
        [Space]
        [Header("Ground Check")]
        [SerializeField] float groundCheckRadius = 0.2f;
        [SerializeField] Vector3 groundCheckOffset;
        [SerializeField] LayerMask groundLayer;

        bool isGrounded;

        float ySpeed;
        Quaternion targetRotation;

        float horizontalInput;
        float verticalInput;
        private readonly int locomotionHash = Animator.StringToHash("Move");

        private void Start()
        {
            cameraController = Camera.main.GetComponent<CameraController>();
            characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            MoveInputRelatively();
        }

        private void MoveInputRelatively()
        {
            Vector3 movementInput = new Vector3(horizontalInput, 0f, verticalInput);

            float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
            animator.SetFloat(locomotionHash, moveAmount, movementDampTime, Time.deltaTime);

            Vector3 moveDirection = cameraController.PlanarRotation * movementInput;
            Vector3 velocity = movementSpeed * moveDirection.normalized;

            if(IsGrounded())
            {
                ySpeed = -0.01f;
            }
            else
            {
                ySpeed += Physics.gravity.y * Time.deltaTime;
            }

            velocity.y = ySpeed;
            characterController.Move(velocity * Time.deltaTime);

            if (moveAmount > 0)
            {
                targetRotation = Quaternion.LookRotation(moveDirection);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

        private bool IsGrounded()
        {
            return Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
        }

        public void ReadMovementInput(InputAction.CallbackContext callbackContext)
        {
            horizontalInput = callbackContext.ReadValue<Vector2>().x;
            verticalInput = callbackContext.ReadValue<Vector2>().y;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
        }
    }
}

