using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class UIManager : MonoBehaviour
    {
        public delegate void MovePlayerAction();
        public static event MovePlayerAction OnMovePlayer;

        public void ClickMove()
        {
            OnMovePlayer();
        }
    }
}
