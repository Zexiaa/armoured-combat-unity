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
        private Controls actions;

        private Vector2 cursorPos;
        private bool isCursorOverUI;

        [SerializeField]
        private GameObject moveMarker;

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

            if (Physics.Raycast(ray, out hit, 100) &&
                !isCursorOverUI)
            {
                moveMarker.SetActive(true);
                moveMarker.transform.position = hit.point;
            }
        }
    }
}
