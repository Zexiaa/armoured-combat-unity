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

        private GameObject playerVehicle; 
        private float vehicleMoveRange;

        [Header("Shooting")]
        //[SerializeField]
        //private LineRenderer playerAimLine;

        private bool isHoldingRightMouse = false;
        private GameObject playerTurret;
        private float turretRotSpeed;

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

            moveMarker.SetActive(false);

            playerVehicle = PlayerMovement.Instance.gameObject;
            vehicleMoveRange = PlayerMovement.Instance.maxMoveRange;

            playerTurret = PlayerMovement.Instance.vehicleTurret;
            turretRotSpeed = PlayerMovement.Instance.turretRotSpeed;
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
                    StartCoroutine(AimTowardsMouse(playerTurret, turretRotSpeed));
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
            if (isCursorOverUI)
                return;

            Ray ray = Camera.main.ScreenPointToRay(cursorPos);
            RaycastHit hit;

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

        private IEnumerator AimTowardsMouse(GameObject obj, float speed)
        {
            while (isHoldingRightMouse)
            {
                Ray ray = Camera.main.ScreenPointToRay(cursorPos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Vector3 targetDirection = hit.point - obj.transform.position;
                    Vector3 newDirection = Vector3.RotateTowards(obj.transform.forward, targetDirection, speed * Time.deltaTime, 0.0f);

                    DrawLine(obj.transform.position + newDirection.normalized * 100,
                        obj.transform.position, TurnManager.Instance.playerAimLine);

                    obj.transform.rotation = Quaternion.LookRotation(new Vector3(newDirection.x, 0, newDirection.z));
                }

                yield return null;
            }
        }

        private void DrawLine(Vector3 pos1, Vector3 pos2, LineRenderer line)
        {
            line.SetPositions(new Vector3[2] { pos1, pos2 });
        }
    }
}
