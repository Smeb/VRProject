using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour {
    private float stability = 0.3f;
    private float convergenceSpeed = 200.0f;
    Rigidbody componentRigidbody;

    private void Start()
    {
        componentRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (componentRigidbody.velocity.magnitude < 3)
        {
            Vector3 predictedY = Quaternion.AngleAxis(
            componentRigidbody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / convergenceSpeed,
            componentRigidbody.angularVelocity
        ) * transform.up;

            Vector3 torque = Vector3.Cross(predictedY, Vector3.up);
            componentRigidbody.AddTorque(torque * Mathf.Pow(convergenceSpeed, 2));
        }   
    }
}
