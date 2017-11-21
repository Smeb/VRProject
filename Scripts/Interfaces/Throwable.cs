using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : Property {
    bool throwing = false;

    public void ThrowObject()
    {
        throwing = true;
    }

    private void FixedUpdate()
    {

        if (throwing)
        {
            Rigidbody rigidbody = rigidbody = gameObject.GetComponent<Rigidbody>();

            if (!(owner is WandController))
            {
                Destroy(this);
                return;
            }

            WandController wandOwner = owner as WandController;

            Transform origin = wandOwner.origin;

            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(wandOwner.velocity) * 1.25f;
                rigidbody.angularVelocity = origin.TransformVector(wandOwner.angularVelocity * 0.25f);
            }
            else
            {
                rigidbody.velocity = wandOwner.velocity;
                rigidbody.angularVelocity = wandOwner.angularVelocity * 0.25f;
            }

            rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            owner = null;
            Destroy(this);
        }
    }
}
