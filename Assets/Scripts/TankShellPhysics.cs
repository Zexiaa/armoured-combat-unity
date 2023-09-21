using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class TankShellPhysics : MonoBehaviour
    {
        public static float GravitationalAcceleration = 9.8f;

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

            if (HasCollisionsInTrajectory(transform.position, CalculateTrajectoryPosition(t)))
            {
                rb.velocity = Vector3.zero;
                gameObject.SetActive(false);
                shellInFlight = false;

                Debug.Log("Shell hit " + hitObject);
            }
            else
            {
                rb.MovePosition(newPosition);

                Debug.Log("Distance travelled: " + Vector3.Distance(startPoint, newPosition));
            }
        }

        public void ShootShell(GameObject shooter, Vector3 _launchDirection, float _muzzleVelocity)
        {
            rb.velocity = Vector3.zero;

            ignoreCollision = shooter;

            //setRange = range;
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
            Vector3 position = startPoint + (launchDirection * initialVelocity * time) + (Vector3.down * GravitationalAcceleration * time * time);

            return position;
        }

        private bool HasCollisionsInTrajectory(Vector3 pointA, Vector3 pointB)
        {
            RaycastHit hit;

            if (Physics.Raycast(pointA, pointB, out hit))
            {
                hitObject = hit.transform.gameObject;
                return true;
            }

            return false;
        }
    }
}
