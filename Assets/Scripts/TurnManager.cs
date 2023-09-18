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

        [SerializeField]
        private Button moveButton;
        
        private GameObject playerVehicle;

        private Projector playerRangeProjector;

        public LineRenderer playerAimLine;

        [HideInInspector]
        public ETurnPhase turnPhase;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            playerVehicle = PlayerMovement.Instance.gameObject;

            playerRangeProjector = playerVehicle.transform.GetComponentInChildren<Projector>();
            playerRangeProjector.enabled = false;
            playerAimLine.enabled = false;

            moveButton.gameObject.SetActive(false);

            SwitchToMovePhase();
        }

        public void SwitchToMovePhase()
        {
            turnPhase = ETurnPhase.Move;

            moveButton.gameObject.SetActive(true);

            playerRangeProjector.enabled = true;
        }

        public void SwitchToShootPhase()
        {
            turnPhase = ETurnPhase.Shoot;

            playerAimLine.enabled = true;
        }

        public void ClickMove()
        {
            OnMovePlayer();

            moveButton.gameObject.SetActive(false);
            playerRangeProjector.enabled = false;
        }

        /* 
         * PRIVATE METHODS 
         */
        private void SwitchTurns()
        {
            // Cycle through player and enemies
        }
    }
}
