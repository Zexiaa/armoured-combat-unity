using System.Collections;
using UnityEngine;

namespace TankGame
{
    public abstract class VehicleMovement : MonoBehaviour
    {
        [SerializeField]
        protected float speed = 5;
        
        private Vector3[] path;
        private int destinationIndex;
        
        public void OnDrawGizmos()
        {
            if (path != null)
            {
                for (int i = destinationIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(path[i], Vector3.one);

                    if (i == destinationIndex)
                        Gizmos.DrawLine(transform.position, path[i]);
                    else
                        Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }

        /// <summary>
        /// Function callback that starts coroutine to move vehicle to destination if path found.
        /// </summary>
        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = newPath;
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
            Vector3 currentWaypoint = path[0];
            destinationIndex = 0;

            while (true)
            {
                // When vehicle reaches next waypoint
                if (transform.position == currentWaypoint)
                {
                    destinationIndex++;

                    // Vehicle reached end
                    if (destinationIndex >= path.Length)
                        yield break;

                    currentWaypoint = path[destinationIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                //Debug.Log("Moving to: " + currentWaypoint);
                
                yield return null;
            }
        }
    }
}
