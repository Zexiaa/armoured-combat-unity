using System.Collections;
using UnityEngine;

namespace TankGame
{
    public class PlayerMovement : VehicleMovement
    {
        [SerializeField]
        private GameObject moveMarker;

        void OnEnable()
        {
            UIManager.OnMovePlayer += MovePlayerVehicle;
        }

        void OnDisable()
        {
            UIManager.OnMovePlayer -= MovePlayerVehicle;
        }

        /* 
         * PRIVATE METHODS 
         */
        private void MovePlayerVehicle()
        {
            //StartCoroutine(UpdatePath());
            NavigationSystem.NavigationManager.CalculatePath(transform.position, moveMarker.transform.position, OnPathFound);
        }

        //private IEnumerator UpdatePath()
        //{
        //    //if (Time.timeSinceLevelLoad < .3f) yield return new WaitForSeconds(.3f);

        //    NavigationSystem.NavigationManager.CalculatePath(transform.position, moveMarker.transform.position, OnPathFound);

        //    float sqrMoveThreshold = DistanceMovedThreshold * DistanceMovedThreshold;
        //    Vector3 targetPosOld = moveMarker.transform.position;

        //    while (true)
        //    {
        //        yield return new WaitForSeconds(MinPathUpdateTime);

        //        if ((moveMarker.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
        //        {
        //            NavigationSystem.NavigationManager.CalculatePath(transform.position, moveMarker.transform.position, OnPathFound);
        //            targetPosOld = moveMarker.transform.position;
        //        }
        //    }
        //}
    }
}
