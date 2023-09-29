using System.Collections;
using UnityEngine;

using TankGame.NavigationSystem;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Processors;

namespace TankGame.Vehicles
{
    public abstract class VehicleMovement : MonoBehaviour
    {
        protected const float DistanceMovedThreshold = .5f;
        protected const float MinPathUpdateTime = .2f;

        [SerializeField]
        protected float speed = 5;

        [SerializeField]
        protected float turnDistance = 5;
        
        [SerializeField]
        protected float turnSpeed = 3;

        [SerializeField]
        protected float stopDistance = 3;

        Path path;
        Rigidbody rb;

        private float forward = 1;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void OnDrawGizmos()
        {
            if (path != null)
                path.DrawWithGizmos();
        }

        /// <summary>
        /// Function callback that starts coroutine to move vehicle to destination if path found.
        /// </summary>
        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDistance, stopDistance);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        /*
         * PRIVATE METHODS
         */

        /// <summary>
        /// Move vehicle along calculated path every frame
        /// </summary>
        private IEnumerator FollowPath()
        {
            bool isFollowingPath = true;
            int pathIndex = 0;

            //transform.LookAt(path.lookPoints[0]);
            //Traverse to point
            //PivotVehicleTowards(path.lookPoints[0]);

            // while (notFacingTarget)
            // calculate direction from vehicle
            // if direction is in front half, point towards direction
            // else point back towards direction

            yield return PivotVehicleTowards(path.lookPoints[0]);

            float speedPercent = 1;

            while (isFollowingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

                // When vehicle has passed the next waypoint turn boundary 
                while (path.turnBoundaries[pathIndex].hasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        isFollowingPath = false;
                        break;
                    }
                    
                    pathIndex++;
                }

                // Slow down before end
                if (pathIndex >= path.slowDownIndex && stopDistance > 0)
                {
                    speedPercent = 
                        Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stopDistance);
                        
                    if (speedPercent < 0.01f)
                    {
                        isFollowingPath = false;
                    }
                }

                // Perform vehicle movement and smooth turning
                //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
                //rb.AddForce(forward * transform.forward * Time.deltaTime * speed * speedPercent);
                
                yield return null;
            }

            OnReachedDestination();
        }

        private IEnumerator PivotVehicleTowards(Vector3 lookPoint)
        {
            bool isFacingDirection = false;

            Vector3 lookDir = (path.lookPoints[0] - transform.position).normalized;

            // If waypoint is behind
            if (Vector3.Dot(transform.forward, lookDir) < 0)
            {
                forward = -1;
            }

            float rotationDir = Vector3.SignedAngle(forward * transform.forward, lookDir, Vector3.up);
            rotationDir = rotationDir == 0.0f ? 1 : Mathf.Clamp(rotationDir, -1, 1);

            while (!isFacingDirection)
            {
                rb.AddTorque(transform.up * turnSpeed * rotationDir);

                yield return null;

                if (Vector3.Dot(forward * transform.forward, lookDir) >= 0.98f)
                    isFacingDirection = true;
            }
        }

        protected virtual void OnReachedDestination()
        {
            TurnManager.Instance.SwitchVehiclePhase(TurnManager.ETurnPhase.Shoot);
        }

    }
}
