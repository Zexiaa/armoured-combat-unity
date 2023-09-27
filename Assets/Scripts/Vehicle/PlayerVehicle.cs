using System;
using UnityEngine;

namespace TankGame
{
    public class PlayerVehicle : Vehicle
    {
        //[SerializeField]
        //private Projector moveRangeProjector;

        [SerializeField]
        private LineRenderer gunAimLine;

        //[SerializeField]
        //private float rangingStep = 100.0f;

        //[SerializeField]
        //private float maxAimLineDistance = 100.0f;

        //void Start()
        //{
        //    ResetEnabledStatus();

        //    maxMoveDistance *= NavigationSystem.NavigationGrid.Instance.NodeDiameter; // 1 Grid node diameter is 1m
        //    moveRangeProjector.orthographicSize = maxMoveDistance;
        //}

        void Update()
        {
            if (gunAimLine != null && gunAimLine.enabled)
            {
                Vector3 gunEndPoint = vehicleGun.transform.position + vehicleGun.transform.forward * ranging;
                //gunEndPoint.y = 0;

                gunAimLine.SetPositions(new Vector3[2] { vehicleGun.transform.position, gunEndPoint });
            }
        }

        /*
         * CLASS METHODS
         */

        public override void StartSpotPhase()
        {
            return; //Skip for now
        }

        public override void StartMovePhase()
        {
            return; //Skip for now
        }

        public override void StartShootPhase()
        {
            gunAimLine.enabled = true;
        }

        public override void StartAmmoPhase()
        {
            gunAimLine.enabled = false;

            TurnManager.Instance.NextVehicleTurn(); // Skip for now
        }
    }
}
