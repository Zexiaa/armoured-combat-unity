using UnityEngine;

namespace TankGame
{
    public class PlayerMovement : VehicleMovement
    {
        public static PlayerMovement Instance { get; private set; }

        [Header("Movement")]
        public float maxMoveRange = 10.0f; 

        [SerializeField]
        private GameObject moveMarker;

        [Header("Turret")]
        public GameObject vehicleTurret;
        public float turretRotSpeed = 2.0f;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void Start()
        {
            maxMoveRange *= NavigationSystem.NavigationGrid.Instance.NodeDiameter; // 1 Grid node diameter is 1m
            transform.GetComponentInChildren<Projector>().orthographicSize = maxMoveRange;
        }

        void OnEnable()
        {
            TurnManager.OnMovePlayer += MovePlayerVehicle;
        }

        void OnDisable()
        {
            TurnManager.OnMovePlayer -= MovePlayerVehicle;
        }

        /* 
         * PRIVATE METHODS 
         */

        private void MovePlayerVehicle()
        {
            //TODO check for movement marker 
            NavigationSystem.NavigationManager.CalculatePath(transform.position, moveMarker.transform.position, OnPathFound);
        }

        protected override void OnReachedDestination()
        {
            base.OnReachedDestination();

            TurnManager.Instance.SwitchToShootPhase();
        }
    }
}
