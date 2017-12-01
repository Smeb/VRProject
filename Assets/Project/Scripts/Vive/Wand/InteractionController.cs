using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WandController : Owner 
{
    // Object highlighting and selection
    private HashSet<GameObject> hoveredInteractables = new HashSet<GameObject>();
    private HashSet<GameObject> hoveredContainers = new HashSet<GameObject>();
    private ContainerController closestContainer;
    private bool itemScaledDown;
    // Item ownership mechanisms
    private FixedJoint fixedJoint;

    private bool helpMode;
    private GameObject m_closestItem;
    private GameObject closestItem
    {
        set
        {
            if (value != m_closestItem)
            {
                if (m_closestItem)
                {
                    Unhighlight(m_closestItem);
                }

                m_closestItem = value;
                if (value)
                {
                    Highlight(value);
                }
            }
        }
        get
        {
            return m_closestItem;
        }
    }

    void ScaleItemDown()
    {
        fixedJoint.connectedBody = null;
        float scale = closestContainer.FindItemScale(ownedItem);
        anchor.transform.localScale = new Vector3(scale, scale, scale);
        fixedJoint.connectedBody = ownedItem.GetComponent<Rigidbody>();
    }

    void SetClosestItem()
    {
        if (hoveredInteractables.Count != 0)
        {
            GameObject lastClosestItem = closestItem;
            float minDistance = float.MaxValue;
            float distance;
            GameObject newClosestItem = null;

            foreach (GameObject gameObject in hoveredInteractables)
            {
                if (gameObject == null) continue; // Could happen if an item is checked out
                distance = (gameObject.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    newClosestItem = gameObject;
                }
            }

            closestItem = newClosestItem;
        }
        else
        {
            closestItem = null;
        }
    }

    void SetClosestContainer()
    {
        if (OwnsItem() && hoveredContainers.Count != 0)
        {
            float minDistance = float.MaxValue;
            float distance;

            foreach (GameObject gameObject in hoveredContainers)
            {
                distance = (gameObject.transform.position - ownedItem.GetComponent<Renderer>().bounds.center).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestContainer = gameObject.GetComponent<ContainerController>();
                }
            }
        }
        else
        {
            closestContainer = null;
        }
    }

    public void ContainerDisabled(GameObject container, GameObject item)
    {
        hoveredInteractables.Remove(item);
        hoveredContainers.Remove(container);
    }

    public override void GiveUpObject(Property item)
    {
        if (ownedItem == item)
        {
            anchor.transform.localScale = new Vector3(1, 1, 1);
            fixedJoint.connectedBody = null;
            base.GiveUpObject(item);
        }
    }

    private void GrabItem()
    {
        if (!closestItem)
        {
            return;
        }

        Throwable item = closestItem.GetComponent<Throwable>();

        if (item == null)
        {
            item = closestItem.AddComponent<Throwable>();
        }

        ownedItem = item;
        fixedJoint.connectedBody = ownedItem.GetComponent<Rigidbody>();

        closestItem = null;
    }

    private void ThrowObject()
    {
        ownedItem.GetComponent<Throwable>().ThrowObject(playerController.activeState.forceScale);
    }

    void Highlight(GameObject gameObject)
    {
        TextureMapping textureMapping = gameObject.GetComponent<TextureMapping>();
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (textureMapping)
        {
            textureMapping.references++;
        }
        else
        {
            TextureMapping mapping = gameObject.AddComponent<TextureMapping>();
            mapping.references = 1;
            mapping.previousMaterial = renderer.material;

            if (gameObject.GetComponent<ProductCode>())
            {
                int code = gameObject.GetComponent<ProductCode>().Code;
                if (code >= 49 && code <= 56)
                {
                    renderer.material = TextureController.alternateHighlight;
                }
                else
                {
                    renderer.material = TextureController.supermarketHighlight;
                }
            }
            else
            {
                renderer.material = TextureController.statueHighlight;
            }
        }
    }

    void Unhighlight(GameObject gameObject)
    {
        TextureMapping textureMapping = gameObject.GetComponent<TextureMapping>();
        if (textureMapping == null)
        {
            return;
        }

        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (textureMapping.references == 1)
        {
            renderer.material = textureMapping.previousMaterial;
            Destroy(textureMapping);
        }
        else
        {
            textureMapping.references--;
        }
    }
}
