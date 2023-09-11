using UnityEngine;

namespace TankGame.NavigationSystem
{
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
