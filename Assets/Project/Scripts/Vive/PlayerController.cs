using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject cameraRig;
    public float walkingSpeed;

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

    private void TouchpadUpdateHandler(int index, WandController controller)
    {
        if (touchpadIndex == index)
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

    private void TouchpadPressHandler(int index)
    {
        if (touchpadIndex == -1)
        {
            touchpadIndex = index;
        }
    }

    private void TouchpadReleaseHandler(int index)
    {
        if (index == touchpadIndex)
        {
            touchpadIndex = -1;
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
}
