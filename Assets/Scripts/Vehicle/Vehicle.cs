using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Vehicle : MonoBehaviour
    {
        public bool isPlayerControlled = false;

        [Header("Vehicle Components")]
        public GameObject vehicleHull;
        public GameObject vehicleTurret;
        public GameObject vehicleGun;
        public GameObject gunExitPoint;

        [SerializeField]
        private Projector moveRangeProjector;

        [SerializeField]
        private LineRenderer gunAimLine;

        [SerializeField]
        private float rangingStep = 100.0f;

        private float ranging = 100.0f;
        private float elevationAngle = 0.0f;

        [Header("Attributes")]
        [SerializeField]
        private float maxMoveDistance = 10.0f;

        [SerializeField]
        private float maxAimLineDistance = 100.0f;

        [SerializeField]
        private float turretRotSpeed = 2.0f;

        [SerializeField]
        private float shellExitForce = 1000.0f;

        [Header("Ammunition")]
        [SerializeField]
        private List<TankShell> shells;
        private TankShell nextShell;

        private Dictionary<TankShell.Category, TankShell> shellTypes = new Dictionary<TankShell.Category, TankShell>();

        /*
         * GET METHODS
         */

        public GameObject VehicleRoot { get { return gameObject; } }

        public Vector3 Position { get { return transform.position; } }

        public float MaxMoveDistance { get { return maxMoveDistance; } }

        public float TurretSpeed { get { return turretRotSpeed; } }

        void Awake()
        {
            ResetEnabledStatus();

            foreach (TankShell shell in shells)
            {
                shellTypes.Add(shell.shellCategory, shell);
            }

            shells.Clear();

            ChangeShellType(TankShell.Category.AP);
        }

        void Start()
        {
            maxMoveDistance *= NavigationSystem.NavigationGrid.Instance.NodeDiameter; // 1 Grid node diameter is 1m
            moveRangeProjector.orthographicSize = maxMoveDistance;
        }

        void Update()
        {
            if (gunAimLine != null && gunAimLine.enabled)
            {
                Vector3 gunEndPoint = vehicleGun.transform.position + vehicleGun.transform.forward * maxAimLineDistance;
                gunEndPoint.y = 0;

                gunAimLine.SetPositions(new Vector3[2] {vehicleGun.transform.position, gunEndPoint});
            }
        }

        /*
         * CLASS METHODS
         */

        public void ResetEnabledStatus()
        {
            SetMoveRangeActive(false);
            SetGunAimLineActive(false);
        }

        public void SetMoveRangeActive(bool isEnabled)
        {
            if (moveRangeProjector == null)
                Debug.LogWarning(VehicleRoot.name + "is missing moveRangeProjector!");
            else
                moveRangeProjector.enabled = isEnabled;
        }

        public void SetGunAimLineActive(bool isEnabled)
        {
            if (gunAimLine == null)
                Debug.LogWarning(VehicleRoot.name + "is missing gunAimLine!");
            else
            {
                gunAimLine.enabled = isEnabled;
            }
        }

        public void ChangeShellType(TankShell.Category shellCategory)
        {
            if (!shellTypes.TryGetValue(shellCategory, out nextShell))
                Debug.LogWarning("Failed to load next shell of type: " + shellCategory);
        }

        public void FireGun(GameObject shellObj)
        {
            //Shell shell;
            //if (!shellTypes.TryGetValue(shellCategory, out shell))
            //    return;

            shellObj.SetActive(true);

            shellObj.transform.SetPositionAndRotation(gunExitPoint.transform.position, Quaternion.LookRotation(vehicleGun.transform.forward));

            shellObj.GetComponent<TankShellPhysics>().ShootShell(VehicleRoot, vehicleGun.transform.forward, nextShell.muzzleVelocity);
        }

        public float AdjustRanging(bool isUpwards)
        {
            if (isUpwards)
                ranging += rangingStep;
            else
                ranging -= rangingStep;

            Debug.Log("Set range to: " + ranging);

            SetElevation(nextShell.shellCategory);

            return ranging;
        }

        /*
         * PRIVATE METHODS
         */

        private void SetElevation(TankShell.Category shellCategory)
        {
            TankShell shell;

            if (shellTypes.TryGetValue(shellCategory, out shell))
            {
                elevationAngle = -RangeToElevation(ranging, shell.muzzleVelocity);

                Debug.Log("Set elevation to " + elevationAngle + " degrees.");

                //vehicleGun.transform.rotation = Quaternion.Euler(elevationAngle, 0, 0);

                vehicleGun.transform.localEulerAngles = new Vector3(elevationAngle, 0, 0);
            }
        }

        private float RangeToElevation(float range, float muzzleVelocity)
        {
            // From ballistic trajectory equation
            // angle = 1/2 * arcsin( g * range / (v * v))

            float elevationRad = Mathf.Atan(
                (range * TankShellPhysics.GravitationalAcceleration) / (muzzleVelocity * muzzleVelocity)
                );
            elevationRad = elevationRad / 2f * Mathf.Rad2Deg;

            return elevationRad;
        }
    }
}
