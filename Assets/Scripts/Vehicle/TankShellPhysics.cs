using System;
using UnityEngine;

namespace TankGame.Vehicles
{
    public class TankShellPhysics : MonoBehaviour
    {
        public static Action OnShellCollided;

        //public static float GravitationalAcceleration = 9.8f;
        public static float ShellMaxLifeTime = 10.0f;

        private Rigidbody rb;
        private GameObject ignoreCollision;

        //private float setRange;
        private float initialVelocity;
        private float startTime;
        private Vector3 startPoint;
        private Vector3 launchDirection;

        private bool shellInFlight = false;
  
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            EnableShell(false);
        }

        void FixedUpdate()
        {
            if (!shellInFlight)
                return;

            float t = Time.time - startTime;

            if (t >= ShellMaxLifeTime)
            {
                EnableShell(false);
                shellInFlight = false;

                OnShellCollided();
                return;
            }

            Vector3 newPosition = CalculateTrajectoryPosition(t);

            if (HasCollisionsInTrajectory(transform.position, CalculateTrajectoryPosition(t), out RaycastHit hit))
            {
                EnableShell(false);
                shellInFlight = false;

                Debug.Log("Shell hit " + hit.transform.gameObject);
                Debug.Log("Distance travelled: " + NavigationSystem.NavigationGrid.Instance.GetWorldDistance(startPoint, hit.point));

                OnShellCollided();
            }
            else
            {
                rb.MovePosition(newPosition);

            }
        }

        public void ShootShell(GameObject shooter, Vector3 _launchDirection, float _muzzleVelocity)
        {
            ignoreCollision = shooter;

            initialVelocity = _muzzleVelocity;
            launchDirection = _launchDirection;

            shellInFlight = true;

            startPoint = transform.position;
            startTime = Time.time;

            transform.GetComponent<TrailRenderer>().Clear();
            EnableShell(true);

            RTSCamera.Instance.SetCameraTracking(gameObject);
        }

        /// <summary>
        /// Calculates ballistic trajectory with no drag
        /// </summary>
        private Vector3 CalculateTrajectoryPosition(float time)
        {
            Vector3 position = startPoint + (launchDirection * initialVelocity * time);

            return position;
        }

        private bool HasCollisionsInTrajectory(Vector3 currentPoint, Vector3 nextPoint, out RaycastHit hit)
        {
            return Physics.Raycast(currentPoint, nextPoint - currentPoint, out hit, (nextPoint - currentPoint).magnitude)
                && hit.transform.gameObject != ignoreCollision;
        }

        private void EnableShell(bool enabled)
        {
            transform.GetComponent<MeshRenderer>().enabled = enabled;
            transform.GetComponent<CapsuleCollider>().enabled = enabled;
            transform.GetComponent<TrailRenderer>().emitting = enabled;
        }
    }
}
