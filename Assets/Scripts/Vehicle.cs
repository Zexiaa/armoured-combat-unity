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

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = newPath;
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        IEnumerator FollowPath()
        {
            Vector3 currentWaypoint = path[0];

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;

                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
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
    }
}