using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public Rigidbody rigidbody;

    private bool interacting;
    private WandController attachedController;
    private Transform interactionPoint;
    private Vector3 positionDelta;
    private Quaternion rotationDelta;
    private Vector3 axis;
    private float angle;
    private float rotationFactor = 400f;
    private float velocityFactor = 2000f;

	// Update is called once per frame
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        velocityFactor /= rigidbody.mass;
        rotationFactor /= rigidbody.mass;

        interactionPoint = new GameObject().transform;
	}

    public void BeginInteraction(WandController controller)
    {
        attachedController = controller;
        interactionPoint.position = controller.transform.position;
        interactionPoint.rotation = controller.transform.rotation;
        interactionPoint.SetParent(transform, true);

        interacting = true;
    }

    public void FixedUpdate()
    {
        if (attachedController && interacting)
        {
            positionDelta = attachedController.transform.position - interactionPoint.position;

            this.rigidbody.velocity = positionDelta * velocityFactor * Time.fixedDeltaTime;
            Debug.Log(this.rigidbody.velocity);

            rotationDelta = attachedController.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            angle = (angle > 180) ? angle - 360 : angle;
            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;

        }
    }

    public void EndInteraction(WandController controller)
    {
        if (controller == attachedController)
        {
            attachedController = null;
            interacting = false;
        }
    }

    public bool IsInteracting()
    {
        return interacting;
    }
}
