using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject cameraRig;

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

                m_activeState.UpdateCamera();
            }
        }
    }

    void Awake()
    {
        humanState = new HumanState(cameraRig, humanReferencePosition, supermarketFloor, 1, 1);
        godState = new GodState(cameraRig, godReferencePosition, sceneFloor, 25, 8);
        activeState = humanState;
    }

    private void Update()
    {

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

    private void TouchpadUpdateHandler(int index, Vector2 position)
    {
        if (touchpadIndex == index)
        {
            activeState.UpdateCamera();
        }
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
