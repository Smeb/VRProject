using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour {
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_Controller.Device controller {  get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;

    private HashSet<InteractableObject> hoveredInteractables = new HashSet<InteractableObject>();
    private InteractableObject closestItem;
    private InteractableObject currentItem;
    private Material supermarketMaterial;
    private Material highlightedMaterial;
    
	// Use this for initialization
	void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();
	}
	
    void SetClosestItem()
    {
        if (hoveredInteractables.Count != 0)
        {
            InteractableObject lastClosestItem = closestItem;
            float minDistance = float.MaxValue;
            float distance;
            foreach (InteractableObject interactable in hoveredInteractables)
            {
                distance = (gameObject.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = interactable;
                }
            }
            
            if (supermarketMaterial == null)
            {
                supermarketMaterial = closestItem.gameObject.GetComponent<Renderer>().material;
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
        SetClosestItem();
		if (controller == null)
        {
            Debug.Log("Controller not initialized.");
            return;
        }

        if (controller.GetPressDown(triggerButton))
        {
            
            if (currentItem)
            {
                if (currentItem.IsInteracting())
                {
                    currentItem.EndInteraction(this);
                }
            }

            if (closestItem)
            {
                currentItem = closestItem;
                closestItem.BeginInteraction(this);
            }
        }

        if (controller.GetPressUp(triggerButton))
        {
            Debug.Log("Press up");
            if (currentItem)
            {
                currentItem.EndInteraction(this);
                currentItem = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable)
        {
            hoveredInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable)
        {
            hoveredInteractables.Remove(interactable);
        }
    }
}
