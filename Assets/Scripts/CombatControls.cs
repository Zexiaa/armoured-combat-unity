using System.Collections;
using TankGame.NavigationSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TankGame
{
    public class CombatControls : MonoBehaviour
    {
        private Controls actions;

        private Vector2 cursorPos;
        private bool isCursorOverUI;

        [Header("Movement")]
        [SerializeField]
        private GameObject moveMarker;

        private bool isHoldingRightMouse = false;

        private Vehicle currentVehicle;

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
                PerformIncreaseAction();
            };
            
            actions.gameplay.subtractButton.performed += context =>
            {
                PerformDecreaseAction();
            };

            actions.gameplay.rKey.performed += context =>
            {
                PerformRanging();
            };

            moveMarker.SetActive(false);
        }

        void Update()
        {
            isCursorOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        void OnEnable()
        {
            TurnManager.SwitchVehicleTurn += SetCurrentVehicle;
        }

        void OnDisable()
        {
            TurnManager.SwitchVehicleTurn -= SetCurrentVehicle;
        }

        public void SetCurrentVehicle()
        {
            currentVehicle = TurnManager.Instance.currentVehicle;
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

        private void PerformRanging()
        {
            //TODO if there is a vehicle close to cursor, range to vehicle
            //TODO Show range as text at cursor position

            Ray ray = Camera.main.ScreenPointToRay(cursorPos);
            RaycastHit hit;

            float dist = 0;

            if (Physics.Raycast(ray, out hit, 100))
                dist = NavigationGrid.Instance.GetWorldDistance(hit.point, currentVehicle.Position);

            Debug.Log("Range to cursor: " +  dist);
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
                //TODO set current vehicle outside
                float dist = NavigationGrid.Instance.GetWorldDistance(hit.point, TurnManager.Instance.currentVehicle.Position);
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
