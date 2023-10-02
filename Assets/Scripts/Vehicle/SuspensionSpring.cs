using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Vehicles
{
    public class SuspensionSpring : MonoBehaviour
    {
        [SerializeField]
        private float springTravel = 0.5f;

        [SerializeField]
        private float springRestLength = 0.5f;

        [SerializeField]
        private float springStiffness = 1.0f;

        [SerializeField]
        private float springDamper = 0.1f;

        [SerializeField]
        private LayerMask terrainMask;

        private float minSpringLength;
        private float maxSpringLength;
        private float springLength;
        private float lastLength;
        private float springForce;
        private float springVelocity;
        private float damperForce;

        private Rigidbody rb;

        //[SerializeField]
        //private GameObject groundContactPrefab;
        //private GameObject groundContact;

        void Start()
        {
            rb = transform.parent.root.GetComponent<Rigidbody>();

            minSpringLength = springRestLength - springTravel;    
            maxSpringLength = springRestLength + springTravel;

            //groundContact = Instantiate(groundContactPrefab, transform.position - transform.up * maxSpringLength, transform.rotation, transform.parent.root);
        }

        void FixedUpdate()
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxSpringLength, terrainMask))
            {
                //Debug.Log(hit.transform.name);

                springLength = Mathf.Clamp(hit.distance, minSpringLength, maxSpringLength);
                springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;

                //groundContact.transform.SetPositionAndRotation(transform.position - transform.up * springLength,
                //    transform.rotation);

                springForce = springStiffness * springRestLength;
                damperForce = springDamper * springVelocity;

                rb.AddForceAtPosition((springForce + damperForce) * transform.up, hit.point);
                
                lastLength = springLength;
            }    
        }
    }
}
