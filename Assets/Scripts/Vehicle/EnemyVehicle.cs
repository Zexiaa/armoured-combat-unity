using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Vehicles
{
    public class EnemyVehicle : Vehicle
    {
        private EnemyMovement movement;

        [SerializeField]
        private float distanceLimitFromPlayers = 10.0f;

        protected override void Awake()
        {
            base.Awake();

            movement = GetComponent<EnemyMovement>();
        }

        private void Start()
        {
            distanceLimitFromPlayers *= NavigationSystem.NavigationGrid.Instance.NodeDiameter;
        }

        public override void StartSpotPhase()
        {
            return; // Skip for now
        }

        public override void StartMovePhase()
        {
            // Get random grid to move to
            movement.MoveVehicle(GetRandomMovePoint());
        }

        public override void StartShootPhase()
        {
            TurnManager.Instance.NextVehicleTurn(); // Skip for now
        }

        public override void StartAmmoPhase()
        {
            TurnManager.Instance.NextVehicleTurn(); // Skip for now
        }

        /*
         * PRIVATE METHODS
         */

        private Vector3 GetRandomMovePoint()
        {
            GameObject player = GetClosestPlayer();
            Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);

            Vector2 vehiclePos2D = new Vector2(transform.position.x, transform.position.z);
            Vector2 randomPosition = vehiclePos2D + Random.insideUnitCircle * MaxMoveDistance;

            if (Vector2.Distance(randomPosition, playerPos2D) < distanceLimitFromPlayers)
            {
                randomPosition = vehiclePos2D + (vehiclePos2D - playerPos2D).normalized * MaxMoveDistance;
            }

            return new Vector3(randomPosition.x, transform.position.y, randomPosition.y);
        }

        private GameObject GetClosestPlayer()
        {
            GameObject[] players = TurnManager.Instance.playerVehicles;

            GameObject closestEnemy = players[0];

            if (players.Length == 1)
            {
                return closestEnemy;
            }

            float minDist = Vector3.Distance(transform.position, closestEnemy.transform.position);
            
            for (int i = 1; i < players.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, players[i].transform.position);
                
                if (dist < minDist)
                {
                    minDist = dist;
                    closestEnemy = players[i];
                }
            }

            return closestEnemy;
        }
    }
}
