using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour {
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_Controller.Device controller {  get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;

    private HashSet<GameObject> hoveredInteractables = new HashSet<GameObject>();
    public FixedJoint fixedJoint;

    [SerializeField]
    private GameObject closestItem;

    private bool throwing;
    private GameObject heldItem;
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

        if (heldItem == null)
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
            if (heldItem)
            {
                ReleaseObject();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            hoveredInteractables.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            hoveredInteractables.Remove(other.gameObject);
        }
    }

    private void GrabObject(GameObject gameObject)
    {
        heldItem = gameObject;
        heldItem.GetComponent<Renderer>().material = supermarketMaterial;

        InteractionController itemController = gameObject.GetComponent<InteractionController>();
        if (itemController == null)
        {
            itemController = gameObject.AddComponent<InteractionController>();
        }

        itemController.GrabObject(this);
        fixedJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
    }

    private void ReleaseObject()
    {
        heldItem.GetComponent<InteractionController>().ReleaseObject();
        GiveUpObject();
    }

    public void GiveUpObject()
    {
        fixedJoint.connectedBody = null;
        heldItem = null;
    }
}
