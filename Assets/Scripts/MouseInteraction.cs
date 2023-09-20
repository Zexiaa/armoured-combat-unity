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

        [Header("Movement")]
        [SerializeField]
        private GameObject moveMarker;

        private bool isHoldingRightMouse = false;

        void Start()
        {
            actions = new Controls();
            actions.Enable();

            actions.gameplay.leftClick.performed += context =>
            {
                PerformLeftClick();
            };

            actions.gameplay.rightHold.performed += context =>
            {
                isHoldingRightMouse = true;
                PerformRightHold();
            };

            actions.gameplay.rightHold.canceled += context =>
            {
                isHoldingRightMouse = false;
            };

            actions.gameplay.cursorPos.performed += context =>
            {
                cursorPos = context.ReadValue<Vector2>();
            };

            actions.gameplay.addButton.performed += context =>
            {
                PerformDecreaseAction();
            };
            
            actions.gameplay.subtractButton.performed += context =>
            {
                PerformIncreaseAction();
            };

            moveMarker.SetActive(false);
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

                case ETurnPhase.Shoot:
                    return;

                default:
                    return;
            }
        }

        private void PerformRightHold()
        {
            switch (TurnManager.Instance.turnPhase)
            {
                case ETurnPhase.Shoot:
                    Vehicle currentVehicle = TurnManager.Instance.currentVehicle;

                    StartCoroutine(AimObjectTowardsCursor(currentVehicle.vehicleTurret, currentVehicle.TurretSpeed));
                    return;

                default:
                    return;
            }
        }

        private void PerformDecreaseAction()
        {
            //TODO set ranging in UI

            TurnManager.Instance.currentVehicle.AdjustRanging(isUpwards: false);
        }

        private void PerformIncreaseAction()
        {
            //TODO set ranging in UI

            TurnManager.Instance.currentVehicle.AdjustRanging(isUpwards: true);
        }

        /// <summary>
        /// Sets move marker to click position
        /// </summary>
        private void SetMoveMarker()
        {
            if (isCursorOverUI)
                return;

            Ray ray = Camera.main.ScreenPointToRay(cursorPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                float dist = Vector3.Distance(hit.point, TurnManager.Instance.currentVehicle.Position) * NavigationGrid.Instance.NodeDiameter;
                float maxDist = TurnManager.Instance.currentVehicle.MaxMoveDistance;

                if (dist < maxDist)
                {
                    moveMarker.SetActive(true);
                    moveMarker.transform.position = hit.point;
                }
                else
                    Debug.Log($"Click point too far ({dist} > {maxDist})!");
            }
        }

        private IEnumerator AimObjectTowardsCursor(GameObject obj, float speed)
        {
            while (isHoldingRightMouse)
            {
                Ray ray = Camera.main.ScreenPointToRay(cursorPos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Vector3 targetDirection = hit.point - obj.transform.position;
                    Vector3 newDirection = Vector3.RotateTowards(obj.transform.forward, targetDirection, speed * Time.deltaTime, 0.0f);

                    obj.transform.rotation = Quaternion.LookRotation(new Vector3(newDirection.x, 0, newDirection.z));
                }

                yield return null;
            }
        }
    }
}
