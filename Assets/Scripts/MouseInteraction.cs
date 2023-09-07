using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TankGame
{
    public class MouseInteraction : MonoBehaviour
    {
        Controls actions;

        private Vector2 cursorPos;

        private bool isCursorOverUI;

        [SerializeField]
        private GameObject moveDestination;

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

            //moveDestination.SetActive(false);
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
                    MarkMovementPoint();
                    return;

                default:
                    return;
            }
        }

        private void MarkMovementPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(cursorPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100) &&
                !isCursorOverUI)
            {
                moveDestination.SetActive(true);
                moveDestination.transform.position = hit.point;
            }
        }
    }
}
