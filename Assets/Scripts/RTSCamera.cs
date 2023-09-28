using System;
using System.Collections;
using UnityEngine;

namespace TankGame
{
    // Referenced from OneWheelStudio (https://github.com/onewheelstudio)
    public class RTSCamera : MonoBehaviour
    {
        public static RTSCamera Instance { get; private set; }

        Controls actions;

        /* Screen Position Tracking */
        private Vector2 cursorPos;

        [SerializeField, Range(0f, 1f)]
        private float screenEdgeThreshold = 0.1f;

        public bool shellTrackingEnabled = true;
        private bool isTracking = false;

        /* Camera Movement Calculation */
        private Vector3 targetPos;

        private Vector3 movementVelocity;
        private Vector3 cameraLastPos;

        private float movementSpeed = 0.0f;

        [SerializeField]
        private float acceleration = 2.0f;

        [SerializeField]
        private float deceleration = 2.0f;

        [SerializeField]
        private float maxSpeed = 1.0f;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void Start()
        {
            actions = new Controls();
            actions.Enable();
            
            actions.gameplay.cursorPos.performed += context =>
            {
                cursorPos = context.ReadValue<Vector2>();
            };
        }

        void OnEnable()
        {
            Vehicles.TankShellPhysics.OnShellCollided += () =>
            {
                isTracking = false;
            };
        }

        void OnDisable()
        {
            Vehicles.TankShellPhysics.OnShellCollided -= () =>
            {
                isTracking = false;
            };
        }

        void Update()
        {
            CheckCursorPosition();

            CalculateCameraVelocity();
            MoveCameraPosition();
            
        }

        public void SetCameraTracking(GameObject obj)
        {
            if (!shellTrackingEnabled) return;

            isTracking = true;

            StartCoroutine(CameraFollow(obj));
        }

        /*
         * PRIVATE METHODS
         */

        private IEnumerator CameraFollow(GameObject obj)
        {
            //Vector3 initialPos = transform.position;

            RaycastHit hit;
            while (isTracking)
            {
                //TODO only include ground layer
                if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
                {
                    Vector3 trackingPosition = obj.transform.position + (transform.position - hit.point);

                    transform.position = Vector3.MoveTowards(
                        transform.position, trackingPosition, acceleration * Time.deltaTime);
                }

                yield return null;
            }

            isTracking = false;
        }

        private void CalculateCameraVelocity()
        {
            movementVelocity = (transform.position - cameraLastPos) / Time.deltaTime;
            movementVelocity.y = 0f;
            cameraLastPos = transform.position;
        }

        private void CheckCursorPosition()
        {
            //TODO Check if game tabbed out?

            Vector3 moveDirection = Vector3.zero;

            // Horizontal Movement
            if (cursorPos.x < screenEdgeThreshold * Screen.width)
            {
                moveDirection -= GetCameraRight();
            }
            else if (cursorPos.x > (1f - screenEdgeThreshold) * Screen.width)
            {
                moveDirection += GetCameraRight();
            }

            // Vertical Movement
            if (cursorPos.y < screenEdgeThreshold * Screen.height)
            {
                moveDirection -= GetCameraForward();
            }
            else if (cursorPos.y > (1f - screenEdgeThreshold) * Screen.height)
            {
                moveDirection += GetCameraForward();
            }

            if (isTracking && moveDirection != Vector3.zero)
                isTracking = false;

            targetPos += moveDirection;
        }

        private Vector3 GetCameraRight()
        {
            Vector3 right = transform.right;
            right.y = 0;
            return right;
        }

        private Vector3 GetCameraForward()
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            return forward;
        }

        private void MoveCameraPosition()
        {
            if (targetPos.sqrMagnitude > 0.1f)
            {
                movementSpeed = Mathf.Lerp(movementSpeed, maxSpeed, Time.deltaTime * acceleration);
                transform.position += targetPos * movementSpeed * Time.deltaTime;
            }
            else
            {
                movementVelocity = Vector3.Lerp(movementVelocity, Vector3.zero, Time.deltaTime * deceleration);
                transform.position += movementVelocity * Time.deltaTime;
            }

            targetPos = Vector3.zero;
        }

        
    }
}
