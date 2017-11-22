using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : Owner {
    public float highlightAlpha = 0.35f;
    private Color originalColor;
    private Color highlightColor;
    private FixedJoint fixedJoint;
    private HashSet<WandController> nearbyControllers;

    private void Start()
    {
        fixedJoint = gameObject.GetComponent<FixedJoint>();

        originalColor = gameObject.GetComponent<Renderer>().material.color;
        highlightColor = new Color(originalColor.r, originalColor.g, originalColor.b, highlightAlpha);
        nearbyControllers = new HashSet<WandController>();
    }

    private void Update()
    {
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        foreach (WandController controller in nearbyControllers)
        {
            if (ControllerCanInteract(controller))
            {
                gameObject.GetComponent<Renderer>().material.color = highlightColor;
                return;
            }
        }
        gameObject.GetComponent<Renderer>().material.color = originalColor;
    }

    protected override void TakeOwnership(Property item)
    {
        base.TakeOwnership(item);

        item.transform.rotation = transform.rotation;
        Vector3 offset = item.GetComponent<Renderer>().bounds.center - item.transform.position;
        item.transform.position = transform.position - offset;

        fixedJoint.connectedBody = item.GetComponent<Rigidbody>();
    }

    public override void GiveUpObject(Property item)
    {
        fixedJoint.connectedBody = null;
        base.GiveUpObject(item);
    }

    private bool ControllerCanInteract(WandController controller)
    {
        return controller.OwnsItem() && !OwnsItem();
    }

    private void OnTriggerEnter(Collider other)
    {
        WandController controller = other.gameObject.GetComponent<WandController>();
        if (controller)
        {
            nearbyControllers.Add(controller);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WandController controller = other.gameObject.GetComponent<WandController>();
        if (controller)
        {
            nearbyControllers.Remove(controller);
        }
    }
}
