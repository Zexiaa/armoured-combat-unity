using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class PlayerMovement : VehicleMovement
    {
        [SerializeField]
        private GameObject moveMarker;

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
            NavigationSystem.NavigationManager.CalculatePath(transform.position, moveMarker.transform.position, OnPathFound);
        }
    }
}
