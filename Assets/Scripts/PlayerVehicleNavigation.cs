using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankGame
{
    [Obsolete()]
    public class PlayerVehicleNavigation : MonoBehaviour
    {

        NavMeshAgent agent;

        [SerializeField]
        private GameObject moveDestination;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
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
            agent.destination = moveDestination.transform.position;
        }
    }
}
