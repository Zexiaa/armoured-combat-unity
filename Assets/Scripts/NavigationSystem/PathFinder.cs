using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TankGame.NavigationSystem
{
    /// <summary>
    /// Component for calculating path
    /// <para>
    /// Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    /// </para>
    /// </summary>
    [RequireComponent(typeof(NavigationGrid))]
    [RequireComponent(typeof(NavigationManager))]
    public class PathFinder : MonoBehaviour
    {
        private NavigationGrid grid;
        private NavigationManager navManager;

        void Awake()
        {
            grid = GetComponent<NavigationGrid>();
            navManager = GetComponent<NavigationManager>();
        }

        /// <summary>
        /// Starts coroutine for pathfinding
        /// </summary>
        public void StartFindPath(Vector3 startPosition, Vector3 endPosition)
        {
            StartCoroutine(AStarPathfind(startPosition, endPosition));
        }

        /*
         * PRIVATE METHODS
         */

        /// <summary>
        /// Calculate path using A* algorithm
        /// </summary>
        private IEnumerator AStarPathfind(Vector3 startPosition, Vector3 endPosition)
        {
            GridNode startNode = grid.WorldPositionToNode(startPosition);
            GridNode endNode = grid.WorldPositionToNode(endPosition);
            
            if (!startNode.walkable && !endNode.walkable)
                yield break;

            Heap<GridNode> openNodes = new Heap<GridNode>(grid.MaxSize);
            HashSet<GridNode> doneNodes = new HashSet<GridNode>();
            openNodes.Add(startNode);

            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            while (openNodes.Count > 0)
            {
                GridNode currentNode = openNodes.RemoveFirst();

                doneNodes.Add(currentNode);

                // When node calculation reaches end destination, stop loop
                if (currentNode == endNode)
                {
                    pathSuccess = true;
                    break;
                }

                // Calculate neighbouring node costs
                foreach (GridNode neighbour in grid.GetNodeNeighbours(currentNode))
                {
                    if (!neighbour.walkable || doneNodes.Contains(neighbour)) continue;

                    int newMoveCostToNeighbour = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbour);
                    if (newMoveCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                    {
                        neighbour.gCost = newMoveCostToNeighbour;
                        neighbour.hCost = GetDistanceBetweenNodes(neighbour, endNode);
                        neighbour.parent = currentNode;
                        
                        if (!openNodes.Contains(neighbour)) openNodes.Add(neighbour);
                    }
                }
            }

            yield return null;

            // Get the shortest path waypoints
            if (pathSuccess)
                waypoints = RetracePath(startNode, endNode);

            navManager.FinishedCalculatingPath(waypoints, pathSuccess);
        }

        /// <summary>
        /// Collate path waypoints between end destination to start position
        /// </summary>
        /// <returns>Waypoint nodes to end destination</returns>
        private Vector3[] RetracePath(GridNode startNode,  GridNode endNode)
        {
            List<GridNode> path = new List<GridNode>();
            GridNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            // Simplify waypoint path
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);

            return waypoints;
        }

        /// <summary>
        /// Removes redundant waypoints in the middle of a straight paths.
        /// </summary>
        /// <param name="path">Calculated grid node path</param>
        /// <returns>Array of world position waypoints</returns>
        private Vector3[] SimplifyPath(List<GridNode> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(
                    path[i - 1].xCoord - path[i].xCoord,
                    path[i - 1].yCoord - path[i].yCoord
                    );

                // If still a straight line don't add waypoint
                // else extend straight line
                if (directionNew != directionOld)
                {
                    Debug.Log("Adding waypoint at " + path[i].worldPosition);
                    waypoints.Add(path[i].worldPosition);
                }
                
                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        /// <summary>
        /// Calculate A* path cost to node
        /// </summary>
        /// <param name="nodeA">Start grid node</param>
        /// <param name="nodeB">Grid node to calculate to</param>
        /// <returns>Distance int</returns>
        private int GetDistanceBetweenNodes(GridNode nodeA, GridNode nodeB)
        {
            int distX = Mathf.Abs(nodeA.xCoord - nodeB.xCoord);
            int distY = Mathf.Abs(nodeA.yCoord - nodeB.yCoord);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
