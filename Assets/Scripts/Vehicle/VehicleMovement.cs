using System.Collections;
using System.Collections.Generic;
using TankGame.NavigationSystem;
using UnityEngine;

namespace TankGame.Vehicles
{
    [System.Serializable]
    public struct TankAxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool isFront;
    }

    public abstract class VehicleMovement : MonoBehaviour
    {
        protected const float DistanceMovedThreshold = .5f;
        protected const float MinPathUpdateTime = .2f;

        [SerializeField]
        private List<TankAxleInfo> axleInfos;

        [SerializeField]
        private float maxTorque = 40;

        [SerializeField]
        private float brakeTorque = 100;

        [SerializeField]
        protected float turnDistance = 5;

        [SerializeField]
        protected float stopDistance = 3;

        Path path;
        Rigidbody rb;

        private float forwardRatio;

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
            float speedPercent = 1;

            while (isFollowingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

                /*
                 * When vehicle has passed the next waypoint turn boundary 
                 */
                if (path.turnBoundaries[pathIndex].hasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                        isFollowingPath = false;
                    else
                        pathIndex++;
                }

                /*
                 * Slow down before end
                 */
                if (pathIndex >= path.slowDownIndex && stopDistance > 0)
                {
                    speedPercent = Vector3.Distance(transform.position, path.lookPoints[path.finishLineIndex]);
                    Debug.Log(speedPercent);
                    speedPercent = Mathf.Clamp01(speedPercent / stopDistance);

                    if (speedPercent < 0.2f)
                    {
                        isFollowingPath = false;
                        break;
                    }
                }


                /*
                 * Face vehicle towards direction
                 */
                Vector3 lookDir = (path.lookPoints[pathIndex] - transform.position).normalized;

                // Waypoint is behind
                if (Vector3.Dot(transform.forward, lookDir) < 0)
                    forwardRatio = -1;
                else
                    forwardRatio = 1;

                float angleToDir = Vector3.SignedAngle(forwardRatio * transform.forward, lookDir, Vector3.up);
                float directionModifier = Mathf.Clamp(Mathf.Abs(angleToDir), 0.0f, 10.0f);

                foreach (TankAxleInfo axle in axleInfos)
                {
                    axle.leftWheel.motorTorque = forwardRatio * maxTorque * directionModifier;
                    axle.rightWheel.motorTorque = forwardRatio * maxTorque * directionModifier;
                    axle.leftWheel.brakeTorque = brakeTorque * (1 - speedPercent);
                    axle.rightWheel.brakeTorque = brakeTorque * (1 - speedPercent);

                    if (forwardRatio > 0 && axle.isFront)
                    {
                        axle.leftWheel.steerAngle = angleToDir * speedPercent;
                        axle.rightWheel.steerAngle = angleToDir * speedPercent;

                    }
                    else if (forwardRatio < 0 && !axle.isFront)
                    {
                        axle.leftWheel.steerAngle = angleToDir * speedPercent;
                        axle.rightWheel.steerAngle = angleToDir * speedPercent;
                    }
                }

                yield return null;
            }

            OnReachedDestination();
        }

        protected virtual void OnReachedDestination()
        {
            foreach (TankAxleInfo axle in axleInfos)
            {
                axle.leftWheel.steerAngle = 0;
                axle.rightWheel.steerAngle = 0;
                axle.leftWheel.motorTorque = 0;
                axle.rightWheel.motorTorque = 0;
                axle.leftWheel.brakeTorque = brakeTorque;
                axle.rightWheel.brakeTorque = brakeTorque;
            }

            TurnManager.Instance.SwitchVehiclePhase(TurnManager.ETurnPhase.Shoot);
        }
    }
}
