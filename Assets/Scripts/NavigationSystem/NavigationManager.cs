using System;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.NavigationSystem
{
    // Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager Instance;

        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        PathRequest currentPathRequest;

        private PathFinder pathFinder;

        private bool isProcessingPath;

        struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[], bool> callback;

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

        public static void CalculatePath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);

            Instance.pathRequestQueue.Enqueue(newRequest);
            Instance.TryProcessNext();
        }

        public void FinishedCalculatingPath(Vector3[] path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;

                pathFinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
            }
        }
    }
}
