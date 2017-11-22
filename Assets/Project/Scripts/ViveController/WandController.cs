using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : Owner {
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_Controller.Device controller {  get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;

    private HashSet<GameObject> hoveredInteractables = new HashSet<GameObject>();
    

    private GameObject closestItem;

    [SerializeField]
    private ContainerController container;

    private FixedJoint fixedJoint;

    private Material supermarketMaterial;
    private Material highlightedMaterial;

    
    public Transform origin { get { return trackedObject.origin; } }
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }
    public Vector3 velocity { get { return controller.velocity; } }

	// Use this for initialization
	void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        fixedJoint = GetComponent<FixedJoint>();
	}
	
    void SetClosestItem()
    {
        if (hoveredInteractables.Count != 0)
        {
            GameObject lastClosestItem = closestItem;
            float minDistance = float.MaxValue;
            float distance;
            foreach (GameObject gameObject in hoveredInteractables)
            {
                distance = (gameObject.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = gameObject;
                }
            }
            
            if (supermarketMaterial == null)
            {
                supermarketMaterial = closestItem.GetComponent<Renderer>().material;
                highlightedMaterial = new Material(supermarketMaterial);
                highlightedMaterial.shader = Shader.Find("Custom/OnHoverOutline");
            }

            if (lastClosestItem)
            {
                lastClosestItem.GetComponent<Renderer>().material = supermarketMaterial;
            }
            closestItem.GetComponent<Renderer>().material = highlightedMaterial;
        }
        else
        {
            if (closestItem)
            {
                closestItem.GetComponent<Renderer>().material = supermarketMaterial;
                closestItem = null;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		if (controller == null)
        {
            Debug.Log("Controller not initialized.");
            return;
        }

        if (ownedItem == null)
        {
            SetClosestItem();
        }
        

        if (controller.GetPressDown(triggerButton))
        {
            if (closestItem)
            {
                GrabObject(closestItem);
            }
        }

        if (controller.GetPressUp(triggerButton))
        {
            if (ownedItem)
            {
                if (container && !container.OwnsItem())
                {
                    GiveItem(container);
                }
                else
                {
                    ThrowObject();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            hoveredInteractables.Add(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Inventory"))
        {
            container = other.GetComponent<ContainerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            hoveredInteractables.Remove(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Inventory"))
        {
            container = null;
        }
    }

    private void GrabObject(GameObject gameObject)
    {
        Throwable item = gameObject.GetComponent<Throwable>();
        if (item == null)
        {
            item = gameObject.AddComponent<Throwable>();
        }
        ownedItem = item;
        fixedJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
    }
     
    private void ThrowObject()
    {
        ownedItem.GetComponent<Throwable>().ThrowObject();
    }

    public override void GiveUpObject(Property item)
    {
        if (ownedItem == item)
        {
            fixedJoint.connectedBody = null;
            base.GiveUpObject(item);
        }
    }
}
