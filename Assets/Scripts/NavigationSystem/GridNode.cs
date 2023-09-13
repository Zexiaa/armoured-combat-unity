using UnityEngine;

namespace TankGame.NavigationSystem
{
    /// <summary>
    /// Node object that makes up a grid for pathfinding
    /// <para>
    /// Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    /// </para>
    /// </summary>
    public class GridNode : IHeapItem<GridNode>
    {
        public bool walkable;

        public int movementPenalty;

        public Vector3 worldPosition;

        // Node coords in world grid
        public int xCoord;
        public int yCoord;

        public int gCost; // distance from starting node
        public int hCost; // (heuristic) distance from end node

        public GridNode parent;

        private int heapIndex;

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public GridNode(bool _walkable, Vector3 _worldPosition, int _xCoord, int _yCoord, int _movementPenalty)
        {
            walkable = _walkable;
            worldPosition = _worldPosition;
            xCoord = _xCoord;
            yCoord = _yCoord;
            movementPenalty = _movementPenalty;
        }

        public int CompareTo(GridNode otherNode)
        {
            int compare = fCost.CompareTo(otherNode.fCost);

            // If both fCosts are the same, compare by hCost
            if (compare == 0)
                compare = hCost.CompareTo(otherNode.hCost);

            return -compare;
        }
    }
}
