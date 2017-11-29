using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(FixedJoint))]
public partial class WandController : Owner
{
    private static PlayerController playerController;

    // Valve controller mappings
    private static Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private static Valve.VR.EVRButtonId touchpadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private static Valve.VR.EVRButtonId toggleViewpoint = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    private SteamVR_Controller.Device controller {  get { return SteamVR_Controller.Input((int)trackedObject.index); } }
    private SteamVR_TrackedObject trackedObject;

    // Controller button press timing
    private Timer timer;

    // Item ownership mechanisms
    private FixedJoint fixedJoint;

    // Propagating transforms of tracked controller
    public Transform origin { get { return trackedObject.origin; } }
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }
    public Vector3 velocity { get { return controller.velocity; } }

    public delegate void TouchpadPress(WandController controller);
    public delegate void TouchpadUpdate(WandController controller);

    public event TouchpadPress OnTouchpadPress;
    public event TouchpadPress OnTouchpadRelease;
    public event TouchpadUpdate OnTouchpadUpdate;

    // Object highlighting and selection
    private HashSet<GameObject> hoveredInteractables = new HashSet<GameObject>();
    private HashSet<GameObject> hoveredContainers = new HashSet<GameObject>();
    private ContainerController closestContainer;

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

    void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        timer = new Timer();
        trackedObject = GetComponent<SteamVR_TrackedObject>();
        fixedJoint = GetComponent<FixedJoint>();
        pointer = GetComponent<LaserPointer>();

        CheckForPointerInteractables();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Persistent") return;
        CheckForPointerInteractables();
    }

    private void OnEnable()
    {
        if (pointer != null)
        {
            pointer.PointerIn += OnPointerEnter;
            pointer.PointerOut += OnPointerExit;

            playerController.ShoppingListOpen += OnShoppingListOpen;
            playerController.ShoppingListClose += OnShoppingListClose;
            playerController.ScanModeOn += OnScanModeOn;
            playerController.ScanModeOff += OnScanModeOff;
        }
        playerController.OnChangeState += ChangeStateHandler;
        playerController.RegisterWand(this);
    }

    private void OnDisable()
    {
        if (pointer != null)
        {
            pointer.PointerIn -= OnPointerEnter;
            pointer.PointerOut -= OnPointerExit;

            playerController.ShoppingListOpen -= OnShoppingListOpen;
            playerController.ShoppingListClose-= OnShoppingListClose;
            playerController.ScanModeOn -= OnScanModeOn;
            playerController.ScanModeOff -= OnScanModeOff;
        }
        playerController.OnChangeState -= ChangeStateHandler;
        playerController.DeregisterWand(this);
    }

    private void ChangeStateHandler(CameraState state)
    {
        if (state is HumanState)
        {
            HumanStateChangeHandler();
        }
        else if (state is CameraState)
        {
            GodStateChangeHandler();
        }
    }

	void Update () {
		if (controller == null)
        {
            Debug.Log("Controller not initialized.");
            return;
        }

        TouchpadButtonUpdate();
        SetClosestContainer();

        if (OwnsItem() && closestContainer && !closestContainer.OwnsItem())
        {
            float scale = closestContainer.FindItemScale(ownedItem);
            ownedItem.transform.localScale *= scale;
            ownedItem.transform.position = transform.position - GetComponent<SphereCollider>().center;
        }
        else if (OwnsItem())
        {
            ownedItem.transform.localScale = new Vector3(1, 1, 1);
        }

        // Viewpoint controls
        if (controller.GetPressDown(toggleViewpoint))
        {
            timer.StartTimer(toggleViewpoint);
        }

        // Trigger controls
        TriggerUpdate();

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

    private void TriggerUpdate()
    {
        if (ownedItem == null && !scanModeOn)
        {
            SetClosestItem();
        }

        if (controller.GetPressDown(triggerButton))
        {
            if (shoppingListOpen && scanModeOn && button == null)
            {
                if (currentProduct != null)
                {
                    playerController.OnAddItem(currentProduct.Code);
                }
            }
            else if (button != null)
            {
                button.onClick.Invoke();
            }
            else if (!scanModeOn && closestItem)
            {
                GrabItem();
            }
        }
    }

    public void TouchpadButtonUpdate()
    {
        if (controller.GetTouchUp(touchpadButton))
        {
            if (OnTouchpadRelease != null)
            {
                OnTouchpadRelease(this);
            }
        }

        if (controller.GetTouchDown(touchpadButton))
        {
            if (OnTouchpadPress != null)
            {
                OnTouchpadPress(this);
            }
        }

        if (controller.GetTouch(touchpadButton))
        {
            OnTouchpadUpdate(this);
        }
    }

    public Vector2 GetTouchpadAxis()
    {
        return controller.GetAxis(touchpadButton);
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
                hoveredContainers.Add(other.gameObject);
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
            hoveredInteractables.Remove(other.gameObject);
        }
    }
}
