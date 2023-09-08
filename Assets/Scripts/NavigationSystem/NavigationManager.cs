using System;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.NavigationSystem
{
    /// <summary>
    /// Manager for vehicles to request for pathfinding
    /// <para>
    /// Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    /// </para>
    /// </summary>
    [RequireComponent(typeof(PathFinder))]
    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager Instance;

        private PathFinder pathFinder;

        private PathRequest currentPathRequest;
        private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();

        private bool isProcessingPath;

        /// <summary>
        /// Struct to request for path calculation 
        /// </summary>
        struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[], bool> callback;

            /// <summary>
            /// Pathfinding request message
            /// </summary>
            /// <param name="_start">Start position</param>
            /// <param name="_end">End destination</param>
            /// <param name="_callback">Function callback</param>
            public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
            {
                pathStart = _start;
                pathEnd = _end;
                callback = _callback;
            }
        }

        void Awake()
        {
            Instance = this;    
            pathFinder = GetComponent<PathFinder>();
        }

        /// <summary>
        /// Calculate path to end destination
        /// </summary>
        /// <param name="pathStart">Start position</param>
        /// <param name="pathEnd">End destination</param>
        /// <param name="callback">Function callback</param>
        public static void CalculatePath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            Instance.pathRequestQueue.Enqueue(newRequest);

            Instance.TryProcessNext();
        }

        /// <summary>
        /// Flag path request as completed
        /// </summary>
        /// <param name="path">World positions to get to end destination</param>
        /// <param name="success">Whether navigation is complete</param>
        public void FinishedCalculatingPath(Vector3[] path, bool success)
        {
            // Move to next waypoint
            currentPathRequest.callback(path, success);
            
            isProcessingPath = false;
            TryProcessNext();
        }

        /// <summary>
        /// Process next path request in queue
        /// </summary>
        private void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;

                pathFinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
            }
            else
            {
                Debug.LogWarning("Problem processing next path!");
            }
        }
    }
}
