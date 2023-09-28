using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Vehicles
{
    public abstract class Vehicle : MonoBehaviour
    {
        public bool isPlayerControlled = false;

        [Header("Vehicle Components")]
        public GameObject vehicleHull;
        public GameObject vehicleTurret;
        public GameObject vehicleGun;
        public GameObject gunExitPoint;

        protected float ranging = 100.0f;
        protected float elevationAngle = 0.0f;

        [Header("Ammunition")]
        [SerializeField]
        protected List<TankShell> shells;
        protected TankShell nextShell;
        
        protected Dictionary<TankShell.Category, TankShell> shellTypes = new Dictionary<TankShell.Category, TankShell>();

        [Header("Attributes")]
        [SerializeField]
        protected float maxMoveDistance = 10.0f;

        [SerializeField]
        private float turretRotSpeed = 2.0f;

        /*
         * GET METHODS
         */

        public GameObject VehicleRoot { get { return gameObject; } }

        public Vector3 Position { get { return transform.position; } }

        public float MaxMoveDistance { get { return maxMoveDistance; } }

        public float TurretSpeed { get { return turretRotSpeed; } }

        protected virtual void Awake()
        {
            foreach (TankShell shell in shells)
            {
                shellTypes.Add(shell.shellCategory, shell);
            }

            shells.Clear();

            ChangeShellType(TankShell.Category.AP);
            SetElevation();
        }

        public void ChangeShellType(TankShell.Category shellCategory)
        {
            if (!shellTypes.TryGetValue(shellCategory, out nextShell))
                Debug.LogWarning("Failed to load next shell of type: " + shellCategory);
        }

        public void FireGun(GameObject shellObj)
        {
            shellObj.transform.SetPositionAndRotation(gunExitPoint.transform.position, Quaternion.LookRotation(vehicleGun.transform.forward));

            shellObj.GetComponent<TankShellPhysics>().ShootShell(VehicleRoot, vehicleGun.transform.forward, nextShell.muzzleVelocity);
        }

        public abstract void StartSpotPhase();

        public abstract void StartMovePhase();

        public abstract void StartShootPhase();

        public abstract void StartAmmoPhase();

        public void ChangeRanging(float addValue)
        {
            ranging += addValue;

            SetElevation();
        }

        /*
         *  PRIVATE METHODS
         */

        protected void SetElevation()
        {
            elevationAngle = RangeToElevation(ranging);

            vehicleGun.transform.localEulerAngles = new Vector3(elevationAngle, 0, 0);
        }

        protected float RangeToElevation(float range)
        {
            float elevationRad = Mathf.Atan(vehicleGun.transform.position.y / range);
            elevationRad *= Mathf.Rad2Deg;

            return elevationRad;
        }
    }
}
