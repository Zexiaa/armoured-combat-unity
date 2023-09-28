using UnityEngine;

namespace TankGame.Vehicles
{
    public class PlayerVehicle : Vehicle
    {
        [SerializeField]
        private LineRenderer gunAimLine;

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
