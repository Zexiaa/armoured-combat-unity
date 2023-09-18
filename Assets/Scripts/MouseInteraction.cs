using System.Collections;
using TankGame.NavigationSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TankGame
{
    public class MouseInteraction : MonoBehaviour
    {
        private Controls actions;

        private Vector2 cursorPos;
        private bool isCursorOverUI;

        [SerializeField]
        private GameObject playerVehicle;

        [Header("Movement")]
        [SerializeField]
        private GameObject moveMarker;

        private float vehicleMoveRange;

        void Start()
        {
            actions = new Controls();
            actions.Enable();

            actions.gameplay.leftClick.performed += context =>
            {
                PerformLeftClick();
            };

            actions.gameplay.cursorPos.performed += context =>
            {
                cursorPos = context.ReadValue<Vector2>();
            };

            moveMarker.SetActive(false);

            vehicleMoveRange = playerVehicle.GetComponent<PlayerMovement>().maxMoveRange;
        }

        void Update()
        {
            isCursorOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        /* 
         * PRIVATE METHODS 
         */


        private void PerformLeftClick()
        {
            switch (TurnManager.Instance.turnPhase)
            {
                case ETurnPhase.Move:
                    SetMoveMarker();
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Sets move marker to click position
        /// </summary>
        private void SetMoveMarker()
        {
            Ray ray = Camera.main.ScreenPointToRay(cursorPos);
            RaycastHit hit;

            if (isCursorOverUI)
                return;

            if (Physics.Raycast(ray, out hit, 100))
            {
                float dist = Vector3.Distance(hit.point, playerVehicle.transform.position) * NavigationGrid.Instance.NodeDiameter;
                
                if (dist < vehicleMoveRange)
                {
                    moveMarker.SetActive(true);
                    moveMarker.transform.position = hit.point;
                }
                else
                    Debug.Log($"Click point too far ({dist} > {vehicleMoveRange})!");
            }
        }
    }
}
