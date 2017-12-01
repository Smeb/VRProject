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

    // Propagating transforms of tracked controller
    public Transform origin { get { return trackedObject.origin; } }
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }
    public Vector3 velocity { get { return controller.velocity; } }

    public delegate void TouchpadPress(WandController controller);
    public delegate void TouchpadUpdate(WandController controller);

    public event TouchpadPress OnTouchpadPress;
    public event TouchpadPress OnTouchpadRelease;
    public event TouchpadUpdate OnTouchpadUpdate;

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

            playerController.ToggleShoppingList += OnShoppingListToggle;
            playerController.ToggleScanMode += OnScanModeToggle;
        }

        playerController.OnChangeState += ChangeStateHandler;
        playerController.ToggleHelpMode += OnHelpModeToggle;
        playerController.RegisterWand(this);
    }

    private void OnDisable()
    {
        if (pointer != null)
        {
            pointer.PointerIn -= OnPointerEnter;
            pointer.PointerOut -= OnPointerExit;

            playerController.ToggleShoppingList -= OnShoppingListToggle;
            playerController.ToggleScanMode -= OnScanModeToggle;
        }
        playerController.OnChangeState -= ChangeStateHandler;
        playerController.ToggleHelpMode-= OnHelpModeToggle;
        playerController.DeregisterWand(this);
    }

    private void OnHelpModeToggle(bool newState)
    {
        helpMode = newState;
    }

    private void ChangeStateHandler(CameraState state)
    {
        if (ownedItem)
        {
            ThrowObject();
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
            if (!itemScaledDown)
            {
                itemScaledDown = true;
                ScaleItemDown();
            }        
        }
        else if (OwnsItem() && itemScaledDown)
        {
            itemScaledDown = false;
            anchor.transform.localScale = new Vector3(1, 1, 1);
        }

        // Viewpoint controls
        if (controller.GetPressDown(toggleViewpoint))
        {
            UCL.COMPGV07.Logging.KeyDown();
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
            UCL.COMPGV07.Logging.KeyDown();

            if (shoppingListOpen && scanModeOn && button == null)
            {
                if (currentProduct != null)
                {
                    playerController.OnAddItem(currentProduct);
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

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            UCL.COMPGV07.Logging.KeyDown();

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
        return controller.GetAxis();
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
            hoveredContainers.Remove(other.gameObject);
        }
    }
}
