using UnityEngine;

namespace TankGame.Vehicles
{
    public class EnemyMovement : VehicleMovement
    {
        public void MoveVehicle(Vector3 targetPosition)
        {
            NavigationSystem.NavigationManager.CalculatePath(transform.position, targetPosition, OnPathFound);
        }

    }
}
