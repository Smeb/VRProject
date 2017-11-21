using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : Owner {
    public float highlightAlpha = 0.35f;
    private Color originalColor;
    private Color highlightColor;
    private HashSet<WandController> nearbyControllers;

    private void Start()
    {
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

    private bool ControllerCanInteract(WandController controller)
    {
        if (this.OwnsItem())
        {
            return !controller.OwnsItem();
        }
        else
        {
            return controller.OwnsItem();
        }
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
