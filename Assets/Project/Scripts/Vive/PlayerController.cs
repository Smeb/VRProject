using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject cameraRig;
    public WandController[] wandControllers;

    public GameObject humanReferencePosition, godReferencePosition, sceneFloor, supermarketFloor;

    private CameraState m_activeState;
    private CameraState activeState
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
                m_activeState.UpdateCamera();
                foreach (WandController wandController in wandControllers)
                {
                    wandController.state = m_activeState;
                }
            }
        }
    }
    private CameraState humanState, godState;

    void Start()
    {
        wandControllers = cameraRig.GetComponentsInChildren<WandController>();
        humanState = new HumanState(cameraRig, humanReferencePosition, supermarketFloor, 1);
        godState = new GodState(cameraRig, godReferencePosition, sceneFloor, 25);
        activeState = humanState;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ToggleViewpoint();
        }
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
        Debug.Log(activeState);
    }
}
