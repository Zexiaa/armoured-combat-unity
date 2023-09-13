using UnityEngine;

namespace TankGame.NavigationSystem
{
    /// <summary>
    /// Struct for calculating perpendicular lines from routes between 2 points.
    /// For use to calculate the turn boundaries for objects to start turning towards the next waypoint.
    /// <para>
    /// Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    /// </para>
    /// </summary>
    public struct PathLine 
    {
        const float VerticalLineGradient = 1e5f;

        float gradient;
        float y_intercept;

        Vector2 pointOnLine_1;
        Vector2 pointOnLine_2;

        float gradientPerpendicular;

        bool approachSide;

        public PathLine(Vector2 nextPoint, Vector2 currentTurningPoint)
        {
            float dirX = nextPoint.x - currentTurningPoint.x;
            float dirY = nextPoint.y - currentTurningPoint.y;

            if (dirX == 0)
                gradientPerpendicular = VerticalLineGradient;
            else
                gradientPerpendicular = dirY / dirX;

            if (gradientPerpendicular == 0)
                gradient = VerticalLineGradient;
            else 
                gradient = -1 / gradientPerpendicular;

            y_intercept = nextPoint.y - gradient * nextPoint.x;

            pointOnLine_1 = nextPoint;
            pointOnLine_2 = nextPoint + new Vector2(1, gradient);

            approachSide = false;
            approachSide = GetSide(currentTurningPoint);
        }

        /// <summary>
        /// Determine if a position is past turn boundary line
        /// </summary>
        /// <param name="p">Point to be determined</param>
        public bool hasCrossedLine (Vector2 p)
        {
            return GetSide(p) != approachSide;
        }

        /// <summary>
        /// Calculate distance of input from current point
        /// </summary>
        /// <param name="p">Object position to be determined</param>
        public float DistanceFromPoint(Vector2 p)
        {
            float yInterceptPerpendicular = p.y - gradient * p.x;
            float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
            float intersectY = gradient * intersectX + y_intercept;

            return Vector2.Distance(p, new Vector2(intersectX, intersectY));
        }

        public void DrawWithGizmos(float length)
        {
            Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCentre = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;

            Gizmos.DrawLine(lineCentre - lineDirection * length / 2f, lineCentre + lineDirection * length / 2f);
        }

        /// <summary>
        /// Get the side of a drawn line that a position is at.
        /// In this case the drawn line is the turn boundary line.
        /// </summary>
        /// <param name="point">Position to be compared</param>
        /// <returns>False if the point has passed or is on the line</returns>
        private bool GetSide(Vector2 point)
        {
            // Perform cross products and compare to determine side
            return (point.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (point.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
        }
    }
}
