using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.NavigationSystem
{
    // Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    public class PathFinder : MonoBehaviour
    {
        private NavigationGrid grid;
        private NavigationManager navManager;

        void Awake()
        {
            grid = GetComponent<NavigationGrid>();
            navManager = GetComponent<NavigationManager>();
        }

        public void StartFindPath(Vector3 startPosition, Vector3 endPosition)
        {
            StartCoroutine(AStarPathfind(startPosition, endPosition));
        }

        /*
         * PRIVATE METHODS
         */

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

                if (currentNode == endNode)
                {
                    pathSuccess = true;
                    break;
                }

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

            if (pathSuccess)
                waypoints = RetracePath(startNode, endNode);

            navManager.FinishedCalculatingPath(waypoints, pathSuccess);
        }

        private Vector3[] RetracePath(GridNode startNode,  GridNode endNode)
        {
            List<GridNode> path = new List<GridNode>();
            GridNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);

            return waypoints;
        }

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

                if (directionNew != directionOld)
                    waypoints.Add(path[i].worldPosition);
                
                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

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
