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
        private int obstacleProximityPenalty = 10;

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
        private bool enableGridGizmos = true;

        [SerializeField]
        private Transform playerVehicle;

        [SerializeField]
        private GameObject moveMarker;

        private int penaltyMin = 0;
        private int penaltyMax = 0;

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

        void OnDrawGizmos()
        {
            if (!enableGridGizmos) return;

            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            // Draw grid cubes
            if (grid == null)
                return;

            GridNode playerNodePosition = WorldPositionToNode(playerVehicle.position);
            GridNode moveMarkerNodePosition = WorldPositionToNode(moveMarker.transform.position);

            foreach (GridNode n in grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.red, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));

                Gizmos.color = n.walkable ? Gizmos.color : Color.black;

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

                    Ray ray = new Ray(nodeWorldPosition + Vector3.up * 20, Vector3.down);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 50, walkableMask))
                        walkableTerrainDict.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);

                    if (!walkable) movementPenalty += obstacleProximityPenalty;

                    grid[x, y] = new GridNode(walkable, nodeWorldPosition, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(3);
        }

        /// <summary>
        /// Introduce gradient to terrain penalties to prevent movement on edge of roads
        /// </summary>
        /// <param name="blurSize"></param>
        private void BlurPenaltyMap(int kernelSize)
        {
            if (kernelSize % 2 == 0) kernelSize++;

            int radius = (kernelSize - 1) / 2;
            //float average = (float) 1 / kernelSize;

            // Horizontal pass
            int[,] horizontalPenalties = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++) {
                // First column
                for (int k = -radius; k < radius; k++)
                {
                    int kernelCell = Mathf.Clamp(k, 0, gridSizeX - 1);

                    horizontalPenalties[kernelCell, y] += grid[kernelCell, y].movementPenalty;
                }

                // Other columns
                for (int x = 1; x < gridSizeX; x++)
                {
                    int prevCell = Mathf.Clamp(x - radius, 0, gridSizeX - 1);
                    int newCell = Mathf.Clamp(x + radius, 0, gridSizeX - 1);

                    horizontalPenalties[x, y] = horizontalPenalties[x-1, y] -
                        grid[prevCell, y].movementPenalty + grid[newCell, y].movementPenalty;
                }
            }

            // Vertical pass
            int[,] verticalPenalties = new int[gridSizeX, gridSizeY];

            for (int x = 0;  x < gridSizeX; x++) {
                // First row
                for (int k = -radius; k < radius; k++)
                {
                    int kernelCell = Mathf.Clamp(k, 0, gridSizeX - 1);

                    verticalPenalties[x, kernelCell] += horizontalPenalties[x, kernelCell];
                }

                // Other rows
                for (int y = 1; y < gridSizeY; y++)
                {
                    int prevCell = Mathf.Clamp(y - radius, 0, gridSizeY - 1);
                    int newCell = Mathf.Clamp(y + radius, 0, gridSizeY - 1);

                    verticalPenalties[x, y] = verticalPenalties[x, y-1] -
                        horizontalPenalties[x, prevCell] + horizontalPenalties[x, newCell];

                    grid[x, y].movementPenalty = Mathf.RoundToInt(verticalPenalties[x, y] / (float) (kernelSize * kernelSize));
          
                    if (grid[x, y].movementPenalty > penaltyMax) penaltyMax = grid[x,y].movementPenalty;
                    if (grid[x, y].movementPenalty < penaltyMin) penaltyMin = grid[x,y].movementPenalty;

                }
            }
        }
    }
}
