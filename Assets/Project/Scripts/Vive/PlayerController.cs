using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void AddItemDelegate(GameObject item);

public class PlayerController : MonoBehaviour
{
    public GameObject cameraRig;
    public event Action ShoppingListOpen, ShoppingListClose;
    public event Action ScanModeOn, ScanModeOff;
    public event AddItemDelegate AddItem;

    public void OnAddItem(GameObject item)
    {
        if (AddItem != null)
        {
            AddItem(item);
        }
    }

    public void OnDestroy()
    {
        Debug.Log("Destroy attempted");
    }

    public GameObject playerBody;
    public ContainerController[] inventories;
    
    WandController activeTouchpadController;
    public float walkingSpeed;
    public string locomotion = "walk-in-place";
    Vector3 lastPosition;
    float velocity;

    // Motion controls
    [SerializeField] float headPosition = 1.65f;
    float headOffset = 0.1f;

    public GameObject godReferencePosition, godReferenceFloor, humanReferenceFloor, humanReferencePosition;
    int godScale = 25, godForceScale = 8;

    private CameraState humanState, godState;
    private CameraState m_activeState;
    private int touchpadIndex = -1;

    public delegate void ChangeState(CameraState newState);
    public event ChangeState OnChangeState;

    public CameraState activeState
    {
        get { return m_activeState; }
        set
        {
            if (m_activeState != value)
            {
                if (m_activeState != null)
                {
                    m_activeState.SetActive(false);
                }

                m_activeState = value;
                m_activeState.SetActive(true);

                if (OnChangeState != null)
                {
                    OnChangeState(m_activeState);
                }

                UpdateCamera(m_activeState);
            }
        }
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Persistent") return;

        m_activeState = null;

        humanReferencePosition = GameObject.Find("HumanPosition");
        humanReferenceFloor = GameObject.Find("HumanFloor");
        godReferencePosition = GameObject.Find("GodPosition");
        godReferenceFloor = GameObject.Find("GodFloor");

        SetGodFloorAndPosition(scene);

        if (humanReferenceFloor == null) Debug.LogError("Human floor reference missing");
        if (humanReferencePosition == null) Debug.LogError("Human position reference missing");
        if (godReferenceFloor == null) Debug.LogError("God floor reference missing");
        if (godReferencePosition == null) Debug.LogError("God position reference missing");

        humanState = new HumanState(cameraRig, humanReferencePosition, humanReferenceFloor, 1, 1);
        godState = new GodState(humanReferencePosition, godReferenceFloor, godScale, godForceScale);
        activeState = humanState;
        SetInventories();
        UpdateCamera(activeState);
    }

    void SetInventories()
    {
        foreach (PlayerContainer container in inventories)
        {
            container.ClearItem();
        }
    }

    void SetGodFloorAndPosition(Scene scene)
    {
        // Manually adjust the GodPositionReference and GodFloor dependent on the user's height
        Renderer godPositionRenderer = godReferencePosition.GetComponent<Renderer>();
        if (!godPositionRenderer) godPositionRenderer = godReferencePosition.GetComponentInChildren<Renderer>();
        float godHeight = headPosition * 1.2f;
        float levelOffset = 0f;

        godReferencePosition.transform.localScale = new Vector3(godReferencePosition.transform.localScale.x, godReferencePosition.transform.localScale.y, godReferencePosition.transform.localScale.z * godHeight);
        float halfTableHeight = godPositionRenderer.bounds.size.y / 2;

        if (scene.name == "Supermarket_01")
        {   
            levelOffset = 9.62f;
        }
        godReferencePosition.transform.position = new Vector3(godReferencePosition.transform.position.x, humanReferenceFloor.transform.position.y - godPositionRenderer.bounds.center.y - halfTableHeight - 0.1f - levelOffset, godReferencePosition.transform.position.z);
        godReferenceFloor.transform.position = new Vector3(godReferenceFloor.transform.position.x, godPositionRenderer.bounds.center.y - halfTableHeight - levelOffset, godReferenceFloor.transform.position.z);

        Destroy(GameObject.Find("ROOF"));
    }

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        inventories = GetComponentsInChildren<PlayerContainer>();
    }

    private void Update()
    {
        SteamVR_Camera camera = cameraRig.GetComponentInChildren<SteamVR_Camera>();
        
        // TODO: Adjust for forceScale parameter when moving user
        if (locomotion == "walk-in-place" &&
            activeTouchpadController &&
            camera.head.localPosition.y > headPosition - headOffset)
        {
            float yChange = camera.head.localPosition.y - lastPosition.y;
            float desiredVelocity = Time.deltaTime > 0 ? (Mathf.Abs(yChange)) * 5 / Time.deltaTime : 0;
            velocity = Mathf.Lerp(velocity, desiredVelocity, Time.deltaTime * 5) * activeState.forceScale;
            Vector3 moveDirection = activeTouchpadController.transform.rotation * Vector3.forward;
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
            Vector3 move = moveDirection * velocity * Time.deltaTime;
            cameraRig.transform.position += move;
        }

        lastPosition = camera.head.localPosition;
    }

    public void RegisterWand(WandController controller)
    {
        controller.OnTouchpadPress += TouchpadPressHandler;
        controller.OnTouchpadRelease += TouchpadReleaseHandler;
        controller.OnTouchpadUpdate += TouchpadUpdateHandler;

        HandUIController handUIController = controller.GetComponentInChildren<HandUIController>();
        if (handUIController)
        {
            handUIController.ShoppingListOpen += OnShoppingListOpen;
            handUIController.ShoppingListClose += OnShoppingListClose;
            handUIController.ScanModeOn += OnScanModeOn;
            handUIController.ScanModeOff += OnScanModeOff;
        }
    }

    public void DeregisterWand(WandController controller)
    {
        controller.OnTouchpadPress -= TouchpadPressHandler;
        controller.OnTouchpadRelease -= TouchpadReleaseHandler;
        controller.OnTouchpadUpdate -= TouchpadUpdateHandler;

        HandUIController handUIController = controller.GetComponentInChildren<HandUIController>();
        if (handUIController)
        {
            handUIController.ShoppingListOpen -= OnShoppingListOpen;
            handUIController.ShoppingListClose -= OnShoppingListClose;
            handUIController.ScanModeOn -= OnScanModeOn;
            handUIController.ScanModeOff -= OnScanModeOff;
        }
    }

    private void OnShoppingListOpen()
    {
        if (ShoppingListOpen != null)
        {
            ShoppingListOpen();
        }
    }

    private void OnShoppingListClose()
    {
        if (ShoppingListClose != null)
        {
            ShoppingListClose();
        }
    }

    private void OnScanModeOn()
    {
        if (ScanModeOn != null)
        {
            ScanModeOn();
        }
    }

    private void OnScanModeOff()
    {
        if (ScanModeOff != null)
        {
            ScanModeOff();
        }
    }

    private void TouchpadUpdateHandler(WandController controller)
    {
        if (activeTouchpadController == controller && locomotion == "touchpad")
        {
            Vector2 axis = controller.GetTouchpadAxis();
            if (axis.y > 0.3f || axis.y < -0.3f)
            {
                float step = walkingSpeed * axis.y;
                Vector3 forwards = new Vector3(
                    controller.transform.forward.x, 0, controller.transform.forward.z).normalized;
                Vector3 update = cameraRig.transform.position + step * Time.deltaTime * forwards * m_activeState.forceScale;
                UpdateCamera(update);
            }
        }
    }

    private void UpdateCamera(Vector3 update)
    {
        cameraRig.transform.position = update;
    }

    private void UpdateCamera(CameraState cameraState)
    {
        cameraRig.transform.position = cameraState.position;
        cameraRig.transform.localScale = new Vector3(1, 1, 1) * cameraState.scale;
    }

    private void TouchpadPressHandler(WandController controller)
    {
        if (activeTouchpadController == null)
        {
            activeTouchpadController = controller;
        }
    }

    private void TouchpadReleaseHandler(WandController controller)
    {
        if (activeTouchpadController == controller)
        {
            activeTouchpadController = null;
        }
    }

    public GameObject GetHumanPosition()
    {
        return humanReferencePosition;
    }

    public void ToggleViewpoint()
    {
        if (activeState == humanState)
        {
            activeState = godState;
        }
        else
        {
            activeState = humanState;
        }
    }

    public float SetHeight()
    {
        SteamVR_Camera camera = cameraRig.GetComponentInChildren<SteamVR_Camera>();
        headPosition = camera.head.localPosition.y;
        SetInventories();
        return headPosition;
    }
}
