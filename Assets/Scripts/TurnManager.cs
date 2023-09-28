using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Vehicles;

namespace TankGame
{
    public class TurnManager : MonoBehaviour
    {
        public enum ETurnPhase
        {
            Spot,
            Move,
            Shoot,
            Ammo
        }

        public static TurnManager Instance { get; private set; }

        public delegate void MovePlayerAction();
        public static event MovePlayerAction OnMovePlayer;

        //public static Action SwitchVehicleTurn;

        [HideInInspector]
        public ETurnPhase turnPhase;

        [SerializeField]
        private List<Vehicle> vehicles;
        private int turnCount = -1;

        [HideInInspector]
        public GameObject[] playerVehicles;
        
        [SerializeField]
        private Button moveButton;

        [SerializeField]
        private Button fireButton;

        [SerializeField]
        private GameObject tankShell;

        [SerializeField]
        private Projector moveProjection;

        public Vehicle CurrentVehicle
        {
            get { return vehicles[turnCount]; }
        }

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void Start()
        {
            moveButton.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(false);

            tankShell.SetActive(false);
            moveProjection.enabled = false;

            List<GameObject> players = new List<GameObject>();
            foreach (Vehicle vehicle in vehicles)
            {
                if (!vehicle.isPlayerControlled)
                    continue;

                players.Add(vehicle.VehicleRoot);
            }
            playerVehicles = players.ToArray();

            NextVehicleTurn();
        }

        void OnEnable()
        {
            TankShellPhysics.OnShellCollided += () =>
            {
                SwitchVehiclePhase(ETurnPhase.Ammo);
            };
        }

        void OnDisable()
        {
            TankShellPhysics.OnShellCollided -= () =>
            {
                SwitchVehiclePhase(ETurnPhase.Ammo);
            };
        }

        public void SwitchVehiclePhase(ETurnPhase phase)
        {
            turnPhase = phase;

            Debug.Log("Switched to " +  turnPhase);

            switch (turnPhase)
            {
                case ETurnPhase.Spot:
                    CurrentVehicle.StartSpotPhase();
                    return;

                case ETurnPhase.Move:
                    if (CurrentVehicle.isPlayerControlled)
                    {
                        //controller.SetMoveRangeActive(true);
                        moveButton.gameObject.SetActive(true);

                        moveProjection.transform.position = CurrentVehicle.transform.position;
                        moveProjection.orthographicSize = CurrentVehicle.MaxMoveDistance;
                        moveProjection.enabled = true;
                        
                    }

                    CurrentVehicle.StartMovePhase();
                    return;

                case ETurnPhase.Shoot:
                    if (CurrentVehicle.isPlayerControlled)
                    {
                        //currentVehicle.SetGunAimLineActive(true);
                        fireButton.gameObject.SetActive(true);
                    }

                    CurrentVehicle.StartShootPhase();
                    return;

                case ETurnPhase.Ammo:
                    CurrentVehicle.StartAmmoPhase();
                    return;
            }
        }

        /*
         * BUTTON METHODS
         */

        public void ClickMove()
        {
            OnMovePlayer();

            moveButton.gameObject.SetActive(false);
            moveProjection.enabled = false;

            //TODO allow players to use up full move range
        }

        public void ClickFire()
        {
            if (CurrentVehicle.isPlayerControlled)
                CurrentVehicle.FireGun(tankShell);

            fireButton.gameObject.SetActive(false);            
        }

        public void NextVehicleTurn()
        {
            turnCount = (turnCount + 1) % vehicles.Count;

            Debug.Log("Turn of: " + CurrentVehicle);

            SwitchVehiclePhase(ETurnPhase.Move);
        }

        /* 
         * PRIVATE METHODS 
         */
    }
}
