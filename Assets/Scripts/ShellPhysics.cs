using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class ShellPhysics : MonoBehaviour
    {
        public static float GravitationalAcceleration = 9.8f;

        private Rigidbody rb;
        private GameObject ignoreCollision;

        private float setRange;
        private float initialVelocity;

        private bool hasShellHit = true;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.root.gameObject != ignoreCollision)
            {
                Debug.Log("Shell hit " +  collision.gameObject.name);

                hasShellHit = true;
                rb.velocity = Vector3.zero;
                gameObject.SetActive(false);

                StopCoroutine(MoveShellAlongTrajectory());
            }
        }

        public void ShootShell(GameObject shooter, float range, float muzzleVelocity)
        {
            ignoreCollision = shooter;

            setRange = range;
            initialVelocity = muzzleVelocity;

            hasShellHit = false;

            StartCoroutine(MoveShellAlongTrajectory());
        }

        private IEnumerator MoveShellAlongTrajectory()
        {
            Vector3 startPoint = transform.position;
            float startTime = Time.time;

            while (!hasShellHit)
            {
                float distTravelled = Mathf.Lerp(0, setRange, Time.deltaTime * initialVelocity);

                float height = GetHorizontalTrajectoryPosition(startTime);

                Vector3 newPosition = startPoint + Vector3.forward * distTravelled;
                newPosition += new Vector3(0, height, 0);

                rb.MovePosition(newPosition);

                yield return null;
            }
        }

        /// <summary>
        /// Calculates ballistic trajectory with no drag
        /// </summary>
        public float GetHorizontalTrajectoryPosition(float startTime)
        {
            // y = initial height + x*tan(gun elevation) - (g*x*x) / (2 * initial velocity * cos^2(gun elevation))

            //float y = initialHeight + x * Mathf.Tan(firedElevation);

            //y -= GravitationalAcceleration * x * x / (2 * initialVelocity * )

            float t = Time.time - startTime;

            float y = initialVelocity * t - (GravitationalAcceleration * t * t) / 2;

            return y;
        }
    }
}
