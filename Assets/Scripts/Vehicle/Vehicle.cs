using System.Collections;
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

        [Header("Attributes")]
        [SerializeField]
        private float maxMoveDistance = 10.0f;

        [SerializeField]
        private float maxAimLineDistance = 100.0f;

        [SerializeField]
        private float turretRotSpeed = 2.0f;

        [SerializeField]
        private float shellExitForce = 1000.0f;

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

        public void FireGun(GameObject shell)
        {
            shell.SetActive(true);

            shell.transform.SetPositionAndRotation(gunExitPoint.transform.position, Quaternion.LookRotation(vehicleGun.transform.forward, -shell.transform.forward));

            shell.GetComponent<Rigidbody>().AddForce(vehicleGun.transform.forward * shellExitForce, ForceMode.Force);
        }
    }
}
