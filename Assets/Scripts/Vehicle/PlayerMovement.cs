using UnityEngine;

namespace TankGame.Vehicles
{
    public class PlayerMovement : VehicleMovement
    {
        [SerializeField]
        private GameObject moveMarker;

        void OnEnable()
        {
            TurnManager.OnMovePlayer += MovePlayerVehicle;
        }

        void OnDisable()
        {
            TurnManager.OnMovePlayer -= MovePlayerVehicle;
        }

        /* 
         * PRIVATE METHODS 
         */

        private void MovePlayerVehicle()
        {
            //TODO check for movement marker 
            NavigationSystem.NavigationManager.CalculatePath(transform.position, moveMarker.transform.position, OnPathFound);
        }
    }
}
