using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {
    bool throwing;

    private WandController owner;
    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void GrabObject(WandController controller)
    {
        if (owner != null)
        {
            owner.GiveUpObject();
        }

        owner = controller;
        throwing = false;
    }

    public void ReleaseObject()
    {
        throwing = true;
    }

    private void FixedUpdate()
    {
        if (throwing)
        {
            Transform origin = owner.origin;

            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(owner.velocity) * 1.25f;
                rigidbody.angularVelocity = origin.TransformVector(owner.angularVelocity * 0.25f);
            }
            else
            {
                rigidbody.velocity = owner.velocity;
                rigidbody.angularVelocity = owner.angularVelocity * 0.25f;
            }

            rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            Destroy(this);
        }
    }
}
