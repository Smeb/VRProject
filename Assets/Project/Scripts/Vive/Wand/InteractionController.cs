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

    [SerializeField]
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
    // Item ownership mechanisms
    private FixedJoint fixedJoint;

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
        if (hoveredContainers.Count != 0)
        {
            float minDistance = float.MaxValue;
            float distance;

            foreach (GameObject gameObject in hoveredContainers)
            {
                distance = (gameObject.transform.position - transform.position).sqrMagnitude;
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

    public void CommonInventoryExitedTrigger(GameObject gameObject)
    {
        hoveredContainers.Remove(gameObject);
    }

    public override void GiveUpObject(Property item)
    {
        if (ownedItem == item)
        {
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
                renderer.material = TextureController.supermarketHighlight;
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
