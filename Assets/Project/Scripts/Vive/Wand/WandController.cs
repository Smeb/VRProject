using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WandController : Owner
{
    private static PlayerController playerController;

    // Valve controller mappings
    private static Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private static Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private static Valve.VR.EVRButtonId toggleViewpoint = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    private SteamVR_Controller.Device controller {  get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;
    
    // Button press timing
    private Timer timer;
    
    // Item ownership mechanisms
    private ContainerController container;
    private FixedJoint fixedJoint;

    // Propagating transforms of tracked controller
    public Transform origin { get { return trackedObject.origin; } }
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }
    public Vector3 velocity { get { return controller.velocity; } }

    public delegate void TouchpadPress(int index);
    public delegate void TouchpadUpdate(int index, WandController controller);

    public event TouchpadPress OnTouchpadPress;
    public event TouchpadPress OnTouchpadRelease;
    public event TouchpadUpdate OnTouchpadUpdate;

    // Object highlighting and selection
    private static Dictionary<Material, Material> highlights;
    private HashSet<GameObject> hoveredInteractables = new HashSet<GameObject>();

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
                    SwapTextures(m_closestItem);
                }
                if (value)
                {
                    SwapTextures(value);
                }
                m_closestItem = value;
            }
        }
        get
        {
            return m_closestItem;
        }
    }

    void SwapTextures(GameObject gameObject)
    {
        TextureMapping textureMapping = gameObject.GetComponent<TextureMapping>();
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (textureMapping)
        {
            renderer.material = textureMapping.previousMaterial;
            Destroy(textureMapping);
        }
        else
        {
            Material original = gameObject.GetComponent<Renderer>().material;
            Material highlight;
            try
            {
                highlight = highlights[original];
            }
            catch
            {
                highlight = new Material(original);
                highlight.shader = Shader.Find("Custom/OnHoverOutline");
                highlights[original] = highlight;
            }
            renderer.material = highlight;
            gameObject.AddComponent<TextureMapping>().previousMaterial = original;
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
        fixedJoint.connectedBody = closestItem.GetComponent<Rigidbody>();

        closestItem = null;
    }

    void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        if (highlights == null)
        {
            highlights = new Dictionary<Material, Material>();
        }

        timer = new Timer();
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        fixedJoint = GetComponent<FixedJoint>();
    }

    private void OnEnable()
    {
        playerController.OnChangeState += ChangeStateHandler;
        playerController.RegisterWand(this);
    }

    private void OnDisable()
    {
        playerController.OnChangeState -= ChangeStateHandler;
        playerController.DeregisterWand(this);
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
        }
        else
        {
            closestItem = null;
        }
    }
    
	void Update () {
		if (controller == null)
        {
            Debug.Log("Controller not initialized.");
            return;
        }

        // Touchpad controls
        if (controller.GetTouchUp(touchpadButton))
        {
            if (OnTouchpadRelease != null)
            {
                OnTouchpadRelease((int)trackedObject.index);
            }
        }

        if (controller.GetTouchDown(touchpadButton))
        {
            if (OnTouchpadPress != null)
            {
                OnTouchpadPress((int)trackedObject.index);
            }
        }

        if (controller.GetTouch(touchpadButton))
        {
            OnTouchpadUpdate((int)trackedObject.index, this);
        }

        // Viewpoint controls
        if (controller.GetPressDown(toggleViewpoint))
        {
            timer.StartTimer(toggleViewpoint);
        }


        // Trigger controls
        if (ownedItem == null)
        {
            SetClosestItem();
        }

        if (controller.GetPressDown(triggerButton))
        {
            if (closestItem)
            {
                GrabItem();
            }
        }

        // Conditional Updates
        if (playerController.activeState is HumanState)
        {
            HumanUpdate();
        }
        else if (playerController.activeState is GodState)
        {
            GodUpdate();
        }
    }

    public Vector2 GetTouchpadAxis()
    {
        return controller.GetAxis(touchpadButton);
    }

    private void ChangeStateHandler()
    {
        if (playerController.activeState is HumanState)
        {
            HumanStateChangeHandler();
        }
        else if (playerController.activeState is CameraState)
        {
            GodStateChangeHandler();
        }
    }

    public override void GiveUpObject(Property item)
    {
        if (ownedItem == item)
        {
            fixedJoint.connectedBody = null;
            base.GiveUpObject(item);
        }
    }

    private void ThrowObject()
    {
        ownedItem.GetComponent<Throwable>().ThrowObject(playerController.activeState.forceScale);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerController.activeState is HumanState)
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
        else if (playerController.activeState is GodState)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("GodInteractables"))
            {
                hoveredInteractables.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable") || other.gameObject.layer == LayerMask.NameToLayer("GodInteractables"))
        {
            hoveredInteractables.Remove(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Inventory"))
        {
            container = null;
        }
    }
}
