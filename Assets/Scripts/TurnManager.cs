using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankGame
{
    public enum ETurnPhase
    {
        Spot,
        Move,
        Shoot,
        Ammo
    }

    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public delegate void MovePlayerAction();
        public static event MovePlayerAction OnMovePlayer;

        public static Action SwitchVehicleTurn;

        [SerializeField]
        private List<Vehicle> vehiclesTurnOrder;
        private int turnCount = -1;

        [HideInInspector]
        public Vehicle currentVehicle;

        [SerializeField]
        private Button moveButton;

        [SerializeField]
        private Button fireButton;

        [SerializeField]
        private GameObject tankShell;
        
        [HideInInspector]
        public ETurnPhase turnPhase;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            moveButton.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(false);

            tankShell.SetActive(false);

            NextVehicleTurn();
        }

        public void SwitchToMovePhase()
        {
            turnPhase = ETurnPhase.Move;

            currentVehicle.SetMoveRangeActive(true);

            moveButton.gameObject.SetActive(true);
        }

        public void SwitchToShootPhase()
        {
            turnPhase = ETurnPhase.Shoot;

            currentVehicle.SetGunAimLineActive(true);

            fireButton.gameObject.SetActive(true);
        }

        /*
         * BUTTON METHODS
         */

        public void ClickMove()
        {
            OnMovePlayer();

            moveButton.gameObject.SetActive(false);

            //TODO allow players to use up full move range
            currentVehicle.SetMoveRangeActive(false);
        }

        public void ClickFire()
        {
            currentVehicle.FireGun(tankShell);
        }

        /* 
         * PRIVATE METHODS 
         */
        private void NextVehicleTurn()
        {
            turnCount++;

            currentVehicle = vehiclesTurnOrder[turnCount];

            SwitchVehicleTurn();
            SwitchToMovePhase();
        }
    }
}
