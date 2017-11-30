using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : ContainerController {
    public HashSet<WandController> triggeringObjects = new HashSet<WandController>();

    public void ToggleVisibility(bool toggle)
    {
        GetComponent<Collider>().enabled = toggle;
        foreach(WandController controller in triggeringObjects)
        {
            if (toggle == false)
            {
                controller.CommonInventoryExitedTrigger(this.gameObject);
            }
        }

        GetComponent<Renderer>().enabled = toggle;
        if (ownedItem)
        {
            ownedItem.GetComponent<Renderer>().enabled = toggle;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        WandController controller = other.gameObject.GetComponent<WandController>();
        if (controller)
        {
            triggeringObjects.Add(controller);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        WandController controller = other.gameObject.GetComponent<WandController>();
        if (controller)
        {
            triggeringObjects.Remove(controller);
        }
    }

    public void ClearItem()
    {
        if (ownedItem != null)
        {
            GiveUpObject(ownedItem);
        }
    }
}
