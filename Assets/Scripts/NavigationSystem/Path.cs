using UnityEngine;

namespace TankGame.NavigationSystem
{
    public class Path
    {
        public readonly Vector3[] lookPoints;
        public readonly PathLine[] turnBoundaries;
        public readonly int finishLineIndex;
        public readonly int slowDownIndex;

        public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance, float stopDistance)
        {
            lookPoints = waypoints;
            turnBoundaries = new PathLine[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            // Calculate turning points for each waypoint
            Vector2 previousPoint = Vector3ToVector2(startPos);

            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector2 currentPoint = Vector3ToVector2(lookPoints[i]);
                Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector2 turnBoundaryPoint = i == finishLineIndex ?
                    currentPoint : currentPoint - dirToCurrentPoint * turnDistance;

                turnBoundaries[i] = new PathLine(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDistance);
                previousPoint = currentPoint;
            }

            // Calculate waypoint right before reaching the stop distance to start slowing down
            float distanceFromDestination = 0;

            for (int i = lookPoints.Length - 1; i > 0; i--)
            {
                distanceFromDestination += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
                if (distanceFromDestination > stopDistance)
                {
                    slowDownIndex = i;
                    break;
                }
            }
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (Vector3 p in lookPoints)
            {
                Gizmos.DrawCube(p + Vector3.up, Vector3.one);
            }

            Gizmos.color = Color.white;
            foreach (PathLine l in turnBoundaries)
            {
                l.DrawWithGizmos(10);
            }
        }

        private Vector2 Vector3ToVector2(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
    }
}
