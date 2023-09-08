using System.Collections.Generic;
using UnityEngine;

namespace TankGame.NavigationSystem
{
    /// <summary>
    /// Component to create a Grid system used for navigation.
    /// Place in a Unity GameObject.
    /// <para>
    /// Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    /// </para>
    /// </summary>
    public class NavigationGrid : MonoBehaviour
    {
        [SerializeField] 
        private LayerMask nonWalkableMask;

        [SerializeField]
        private Vector2 gridWorldSize;

        [SerializeField]
        private float nodeRadius;

        [SerializeField]
        private Transform playerVehicle;

        GridNode[,] grid;
        private int gridSizeX, gridSizeY;

        private float nodeDiameter;

        public int MaxSize
        {
            get { return gridSizeX * gridSizeY; }
        }

        void Awake()
        {
            nodeDiameter = nodeRadius * 2;

            // Calculate number of grids for each side
            gridSizeX = Mathf.FloorToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.FloorToInt(gridWorldSize.y / nodeDiameter);

            CreateGrid();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            // Draw grid cubes
            if (grid == null)
                return;

            GridNode playerNodePosition = WorldPositionToNode(playerVehicle.position);

            foreach (GridNode n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;

                if (n == playerNodePosition) Gizmos.color = Color.green;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * nodeDiameter);
            }
        }

        /*
         * PUBLIC METHODS
         */

        /// <summary>
        /// Converts Vector3 world positions into grid coordinates
        /// </summary>
        /// <param name="worldPosition">Position to convert (i.e. player)</param>
        /// <returns><see cref="GridNode"/> array position </returns>
        public GridNode WorldPositionToNode(Vector3 worldPosition)
        {
            // Add half of gridWorldSize length as origin starts in the middle of the grid
            float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            //Debug.Log("Player grid coord: " + percentX + ", " + percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            return grid[x, y];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<GridNode> GetNodeNeighbours(GridNode node)
        {
            List<GridNode> neighbours = new List<GridNode>();

            Debug.Log($"Getting neighbours of node {node.xCoord}, {node.yCoord}");

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {

                    if (x == 0 &&  y == 0) continue;

                    if (node.xCoord + x >= 0 && node.xCoord + x < gridSizeX &&
                    node.yCoord + y >= 0 && node.yCoord + y < gridSizeY)
                        Debug.Log($"Adding neighour {node.xCoord + x}, {node.yCoord + y}");
                        neighbours.Add(grid[node.xCoord + x, node.yCoord + y]);
                }
            }

            return neighbours;
        }

        /*
         * PRIVATE METHODS
         */

        /// <summary>
        /// Creates a 2D array grid of nodes within the <see cref="gridWorldSize"/>
        /// </summary>
        private void CreateGrid()
        {
            grid = new GridNode[gridSizeX, gridSizeY];

            Vector3 worldBottomLeft = 
                transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            // Create grid nodes starting from bottom left
            for (int x = 0; x < gridSizeX; x++) {
                for (int y = 0; y < gridSizeY; y++) {
                    
                    Vector3 nodeWorldPosition = worldBottomLeft +
                        Vector3.right * (x * nodeDiameter + nodeRadius) +
                        Vector3.forward * (y * nodeDiameter + nodeRadius);

                    bool walkable = !Physics.CheckSphere(nodeWorldPosition, nodeRadius, nonWalkableMask);

                    grid[x, y] = new GridNode(walkable, nodeWorldPosition, x, y);
                }
            }
        }
    }
}
