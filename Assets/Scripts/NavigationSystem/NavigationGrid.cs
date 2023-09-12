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
        [Header("Terrain")]
        [SerializeField] 
        private LayerMask nonWalkableMask;

        [SerializeField]
        private TerrainType[] terrainModifiers;
        private LayerMask walkableMask;
        Dictionary<int, int> walkableTerrainDict = new Dictionary<int, int>();

        [Header("Grid Settings")]

        [SerializeField]
        private Vector2 gridWorldSize;

        [SerializeField]
        private float nodeRadius;

        GridNode[,] grid;
        private int gridSizeX, gridSizeY;

        private float nodeDiameter;

        [Header("====Debug====")]

        [SerializeField]
        private Transform playerVehicle;

        [SerializeField]
        private GameObject moveMarker;

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

            foreach (TerrainType terrain in terrainModifiers)
            {
                walkableMask.value |= terrain.terrainMask.value;
                walkableTerrainDict.Add((int) Mathf.Log(terrain.terrainMask.value, 2), terrain.terrainPenalty);
            }

            CreateGrid();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            // Draw grid cubes
            if (grid == null)
                return;

            GridNode playerNodePosition = WorldPositionToNode(playerVehicle.position);
            GridNode moveMarkerNodePosition = WorldPositionToNode(moveMarker.transform.position);

            foreach (GridNode n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;

                if (n == playerNodePosition) Gizmos.color = Color.green;
                if (n == moveMarkerNodePosition) Gizmos.color = Color.yellow;

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

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {

                    if (x == 0 &&  y == 0) continue;

                    if (node.xCoord + x >= 0 && node.xCoord + x < gridSizeX &&
                    node.yCoord + y >= 0 && node.yCoord + y < gridSizeY)
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

                    // Add terrain penalty
                    int movementPenalty = 0;

                    if (walkable)
                    {
                        Ray ray = new Ray(nodeWorldPosition + Vector3.up * 20, Vector3.down);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 50, walkableMask))
                            walkableTerrainDict.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    grid[x, y] = new GridNode(walkable, nodeWorldPosition, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(3);
        }

        /// <summary>
        /// Introduce gradient to terrain penalties to prevent movement on edge of roads
        /// </summary>
        /// <param name="blurSize"></param>
        private void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1; // Must be odd
            int kernelExtents = (kernelSize - 1) / 2; // Number of squares between centre to edge of kernel

            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++) { 
                for (int x = -kernelExtents; x <= kernelExtents; x++) 
                {

                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = 
                        penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }
            
            for (int x = 0; x < gridSizeX; x++) { 
                for (int y = -kernelExtents; y <= kernelExtents; y++) 
                {

                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].movementPenalty = blurredPenalty;

                for (int y = 1; x < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] =
                        penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];

                    blurredPenalty = Mathf.RoundToInt((float) penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].movementPenalty = blurredPenalty;
                }
            }
        }
    }
}
