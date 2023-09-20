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
        private List<Shell> shells;
        private Shell nextShell;

        private Dictionary<Shell.Category, Shell> shellTypes = new Dictionary<Shell.Category, Shell>();

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

            foreach (Shell shell in shells)
            {
                shellTypes.Add(shell.shellCategory, shell);
            }

            shells.Clear();

            ChangeShellType(Shell.Category.AP);
        }

        void Start()
        {
            maxMoveDistance *= NavigationSystem.NavigationGrid.Instance.NodeDiameter; // 1 Grid node diameter is 1m
            moveRangeProjector.orthographicSize = maxMoveDistance;
        }

        void Update()
        {
            if (gunAimLine != null && gunAimLine.enabled)
                gunAimLine.SetPositions(new Vector3[2] {vehicleGun.transform.position,
                    vehicleGun.transform.position + vehicleGun.transform.forward * maxAimLineDistance});
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

        public void ChangeShellType(Shell.Category shellCategory)
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

            shellObj.transform.SetPositionAndRotation(gunExitPoint.transform.position, Quaternion.LookRotation(vehicleGun.transform.forward, -shellObj.transform.forward));

            shellObj.GetComponent<ShellPhysics>().ShootShell(VehicleRoot, ranging, nextShell.muzzleVelocity);
        }

        public float AdjustRanging(bool isUpwards)
        {
            if (isUpwards)
                ranging += rangingStep;
            else
                ranging -= rangingStep;

            SetElevation(nextShell.shellCategory);

            return ranging;
        }

        /*
         * PRIVATE METHODS
         */

        private float SetElevation(Shell.Category shellCategory)
        {
            Shell shell;

            if (shellTypes.TryGetValue(shellCategory, out shell))
            {
                elevationAngle = RangeToElevation(ranging, shell.muzzleVelocity);

                vehicleGun.transform.rotation = Quaternion.Euler(elevationAngle, 0, 0);
            }

            return ranging;
        }

        private float RangeToElevation(float range, float muzzleVelocity)
        {
            // 2 * Theta = arcsin((R * g) / (u * u))

            float elevation = Mathf.Asin(
                (range * ShellPhysics.GravitationalAcceleration) / (muzzleVelocity * muzzleVelocity)
                );

            elevation *= 180f / Mathf.PI;

            return elevation / 2;
        }
    }
}
