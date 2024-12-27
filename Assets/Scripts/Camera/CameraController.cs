using UnityEngine;
using UnityEngine.InputSystem;

namespace ParkourSystem.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        //3rd person Camera Controller

        [SerializeField] Transform followTarget;
        [SerializeField] Vector3 offset;
        [SerializeField] float rotationSpeed = 5f;
        [SerializeField] float minVerticalAngle;
        [SerializeField] float maxVerticalAngle;
        [SerializeField] Vector2 rotationInput;

        [SerializeField] bool invertX;
        [SerializeField] bool invertY;

        float rotationY;
        float rotationX;

        float invertXVal;
        float invertYVal;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            invertYVal = invertY ? -1 : 1;
            invertXVal = invertX ? -1 : 1;

            rotationY -= rotationInput.x * rotationSpeed * invertYVal;
            rotationX += rotationInput.y * rotationSpeed * invertXVal;

            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

            Quaternion camTargetRotation = Quaternion.Euler(rotationX, rotationY, 0);

            //rotate the offset to determine camera position based on input
            Vector3 rotatedOffsetVector = camTargetRotation * offset;
            Vector3 camTargetPosition = followTarget.position + rotatedOffsetVector;

            // Smoothly update position and rotation
            transform.position = camTargetPosition;
            transform.rotation = camTargetRotation;
        }

        public void ReadRotationInput(InputAction.CallbackContext context)
        {
            rotationInput = context.ReadValue<Vector2>().normalized;

            if (context.canceled)
            {
                rotationInput = Vector2.zero;
            }
        }

        public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
    }
}

