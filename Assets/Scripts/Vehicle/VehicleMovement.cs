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

        //[SerializeField]
        //private float maxSteerAngle = 10f;

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

        private float forwardRatio = 1;

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

                // When vehicle has passed the next waypoint turn boundary 
                while (path.turnBoundaries[pathIndex].hasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        isFollowingPath = false;
                        break;
                    }

                    foreach (TankAxleInfo axle in axleInfos)
                    {
                        axle.leftWheel.steerAngle = 0;
                        axle.rightWheel.steerAngle = 0;
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

                // Turn
                Vector3 lookDir = (path.lookPoints[pathIndex] - transform.position).normalized;

                // If waypoint is behind
                if (Vector3.Dot(transform.forward, lookDir) < 0)
                {
                    forwardRatio = -1;
                }

                //float angleToWaypoint = Vector3.Angle(forward * transform.forward, lookDir);
                //angleToWaypoint = Mathf.Clamp(angleToWaypoint, 0, maxSteerAngle);
                //if (angleToWaypoint > maxSteerAngle)
                //    yield return PivotVehicleTowards(lookDir);

                // Perform vehicle movement and smooth turning
                //Quaternion targetRotation = Quaternion.LookRotation((path.lookPoints[pathIndex] - transform.position) * forward);
                //rb.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //transform.Translate(forward * Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);

                //rb.AddForce(forward * transform.forward * speed * speedPercent);

                foreach (TankAxleInfo axle in axleInfos)
                {
                    axle.leftWheel.motorTorque = forwardRatio * maxTorque * speedPercent;
                    axle.rightWheel.motorTorque = forwardRatio * maxTorque * speedPercent;

                    if (forwardRatio > 0 && axle.isFront)
                    {
                        axle.leftWheel.steerAngle = Vector3.SignedAngle(axle.leftWheel.transform.forward, lookDir, Vector3.up);
                        axle.rightWheel.steerAngle = Vector3.SignedAngle(axle.rightWheel.transform.forward, lookDir, Vector3.up);

                    }
                    else if (forwardRatio < 0 && !axle.isFront)
                    {
                        axle.leftWheel.steerAngle = Vector3.SignedAngle(forwardRatio * axle.leftWheel.transform.forward, lookDir, Vector3.up);
                        axle.rightWheel.steerAngle = Vector3.SignedAngle(forwardRatio * axle.rightWheel.transform.forward, lookDir, Vector3.up);
                    }
                }

                yield return null;
            }

            OnReachedDestination();
        }

        //private IEnumerator PivotVehicleTowards(Vector3 lookDir)
        //{
        //    bool isFacingDirection = false;

        //    float rotationDir = Vector3.SignedAngle(forward * transform.forward, lookDir, Vector3.up);
        //    rotationDir = rotationDir >= 0.0f ? 1 : -1;

        //    while (!isFacingDirection)
        //    {
        //        //rb.AddTorque(transform.up * turnSpeed * rotationDir);

        //        foreach (AxleInfo axle in axleInfos)
        //        {
        //            if (forward > 0 && axle.isFront)
        //            {
        //                axle.leftWheel.motorTorque = forward * maxTorque;
        //                axle.rightWheel.motorTorque = forward * maxTorque;
        //                axle.leftWheel.steerAngle = maxSteerAngle * rotationDir;
        //                axle.rightWheel.steerAngle = maxSteerAngle * rotationDir;
        //            }
        //            else if(forward < 0 && !axle.isFront)
        //            {
        //                axle.leftWheel.motorTorque = forward * maxTorque;
        //                axle.rightWheel.motorTorque = forward * maxTorque;
        //                axle.leftWheel.steerAngle = maxSteerAngle * rotationDir;
        //                axle.rightWheel.steerAngle = maxSteerAngle * rotationDir;
        //            }
        //        }

        //        yield return null;

        //        if (Vector3.Dot(forward * transform.forward, lookDir) >= 0.90f)
        //            isFacingDirection = true;
        //    }
        //}

        protected virtual void OnReachedDestination()
        {
            foreach (TankAxleInfo axle in axleInfos)
            {
                axle.leftWheel.motorTorque = 0;
                axle.rightWheel.motorTorque = 0;
                axle.leftWheel.steerAngle = 0;
                axle.rightWheel.steerAngle = 0;
            }

            TurnManager.Instance.SwitchVehiclePhase(TurnManager.ETurnPhase.Shoot);
        }

    }
}
