using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : ContainerController {
    public HashSet<WandController> triggeringObjects = new HashSet<WandController>();
    public delegate void ItemUpdate(GameObject item);
    public ItemUpdate RemoveItem;
    public ItemUpdate AddItem;

    public void ToggleVisibility(bool toggle)
    {
        GetComponent<Collider>().enabled = toggle;

        GameObject containedObject = null;
        GetComponent<Renderer>().enabled = toggle;

        if (ownedItem)
        {
            containedObject = ownedItem.gameObject;
            ownedItem.GetComponent<Renderer>().enabled = toggle;
            ownedItem.GetComponent<Collider>().enabled = toggle;
        }

        foreach (WandController controller in triggeringObjects)
        {
            if (toggle == false)
            {
                controller.ContainerDisabled(this.gameObject, containedObject);
            }
        }

    }

    protected override void TakeOwnership(Property item)
    {
        if (AddItem != null)
        {
            AddItem(item.gameObject);
        }
        base.TakeOwnership(item);
    }

    public override void GiveUpObject(Property item)
    {
        if (RemoveItem != null)
        {
            RemoveItem(item.gameObject);
        }
        base.GiveUpObject(item);
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
