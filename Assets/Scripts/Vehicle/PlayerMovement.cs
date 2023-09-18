using TankGame.NavigationSystem;
using UnityEngine;

namespace TankGame
{
    public class PlayerMovement : VehicleMovement
    {
        public float maxMoveRange = 10.0f; 

        [SerializeField]
        private GameObject moveMarker;

        void Start()
        {
            maxMoveRange *= NavigationGrid.Instance.NodeDiameter; // 1 Grid node diameter is 1m
            transform.GetComponentInChildren<Projector>().orthographicSize = maxMoveRange;
            //transform.GetComponentInChildren<Projector>().enabled = false;
        }

        void OnEnable()
        {
            UIManager.OnMovePlayer += MovePlayerVehicle;
        }

        void OnDisable()
        {
            UIManager.OnMovePlayer -= MovePlayerVehicle;
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
