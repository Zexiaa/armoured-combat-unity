using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class TankShellPhysics : MonoBehaviour
    {
        public static float GravitationalAcceleration = 9.8f;
        public static float ShellMaxLifeTime = 10.0f;

        private Rigidbody rb;
        private GameObject ignoreCollision;

        //private float setRange;
        private float initialVelocity;
        private float startTime;
        private Vector3 startPoint;
        private Vector3 launchDirection;

        private bool shellInFlight = true;

        private GameObject hitObject;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (!shellInFlight)
                return;

            float t = Time.time - startTime;
            Vector3 newPosition = CalculateTrajectoryPosition(t);

            if (HasCollisionsInTrajectory(transform.position, CalculateTrajectoryPosition(t), out RaycastHit hit))
            {
                gameObject.SetActive(false);
                shellInFlight = false;

                Debug.Log("Shell hit " + hit.transform.gameObject);
                Debug.Log("Distance travelled: " + NavigationSystem.NavigationGrid.Instance.GetWorldDistance(startPoint, hit.point));
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
    }
}
