using System.Collections;
using UnityEngine;

namespace TankGame
{
    public class Vehicle : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private float speed = 5;
        
        private Vector3[] path;
        private int targetIndex;
        
        void Start()
        {
            NavigationSystem.NavigationManager.CalculatePath(transform.position, target.position, OnPathFound);
        }

        public void OnDrawGizmos()
        {
            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(path[i], Vector3.one);

                    if (i == targetIndex)
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

            while (true)
            {
                // When vehicle reaches next waypoint
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;

                    // Vehicle reached end
                    if (targetIndex >= path.Length)
                        yield break;

                    currentWaypoint = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                
                yield return null;
            }
        }
    }
}
