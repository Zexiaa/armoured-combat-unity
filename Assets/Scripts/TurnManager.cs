using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [HideInInspector]
        public ETurnPhase turnPhase;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            turnPhase = ETurnPhase.Move;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void OnEnable()
        {
            UIManager.OnMovePlayer += SwitchToShootPhase;
        }

        void OnDisable()
        {
            UIManager.OnMovePlayer -= SwitchToShootPhase;
        }

        /* PRIVATE METHODS */
        private void SwitchTurns()
        {
            // Cycle through player and enemies
        }

        private void SwitchToShootPhase()
        {
            //turnPhase = ETurnPhase.Shoot;
        }
    }
}
