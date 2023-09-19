using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class ShellPhysics : MonoBehaviour
    {
        private Rigidbody rb;
        private GameObject ignoreCollision;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.root.gameObject != ignoreCollision)
            {
                rb.velocity = Vector3.zero;
                gameObject.SetActive(false);
            }
        }

        public void SetFirer(GameObject firer)
        {
            ignoreCollision = firer;
        }
    }
}
