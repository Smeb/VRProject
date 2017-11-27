using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject cameraRig;

    WandController activeTouchpadController;
    public float walkingSpeed;
    string locomotion = "walk-in-place";
    Vector3 lastPosition;
    float velocity;
    float headPosition = 1.65f;

    public GameObject humanReferencePosition, godReferencePosition, sceneFloor, supermarketFloor;

    private CameraState humanState, godState;
    private CameraState m_activeState;
    private int touchpadIndex = -1;

    public delegate void ChangeState();
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
                    OnChangeState();
                }

                UpdateCamera(m_activeState);
            }
        }
    }

    void Awake()
    {
        humanState = new HumanState(cameraRig, humanReferencePosition, supermarketFloor, 1, 1);
        godState = new GodState(godReferencePosition, sceneFloor, 25, 8);
        activeState = humanState;
    }

    private void Update()
    {
        SteamVR_Camera camera = cameraRig.GetComponentInChildren<SteamVR_Camera>();
        if (locomotion == "walk-in-place" && activeTouchpadController)
        {
            var yChange = camera.head.localPosition.y - lastPosition.y;
            var desiredVelocity = Time.deltaTime > 0 ? (Mathf.Abs(yChange)) * 5 / Time.deltaTime : 0;
                velocity = Mathf.Lerp(velocity, desiredVelocity, Time.deltaTime * 5);

                var lookDirection = camera.transform.rotation * Vector3.forward;
                var moveDirection = new Vector3(lookDirection.x, 0, lookDirection.z).normalized;
                var move = moveDirection * velocity * Time.deltaTime;

                transform.position += transform.rotation * move;
        }
        lastPosition = camera.head.localPosition;
    }

    public void RegisterWand(WandController controller)
    {
        controller.OnTouchpadPress += TouchpadPressHandler;
        controller.OnTouchpadRelease += TouchpadReleaseHandler;
        controller.OnTouchpadUpdate += TouchpadUpdateHandler;
    }

    public void DeregisterWand(WandController controller)
    {
        controller.OnTouchpadPress -= TouchpadPressHandler;
        controller.OnTouchpadRelease -= TouchpadReleaseHandler;
        controller.OnTouchpadUpdate -= TouchpadUpdateHandler;
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
        return headPosition;
    }
}
