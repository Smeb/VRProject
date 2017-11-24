using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameObject cameraRig;

    public GameObject godPosition, humanPosition;
    StateData godState, humanState;


    // Use this for initialization

    void Start () {
        humanState = new StateData(humanPosition, true);
        godState = new StateData(godPosition, false);
        UpdateCamera(humanState);
    }
	
	// Update is called once per frame
	void Update () {
        		
	}

    private void UpdateCamera(StateData state)
    {
        cameraRig.transform.position = state.positionObject.transform.position;
        cameraRig.transform.rotation = state.positionObject.transform.rotation;
        cameraRig.transform.localScale = state.positionObject.transform.localScale;
    }

    public void ToggleViewpoint()
    {
        if (humanState.active)
        {
            humanState.SetActive(cameraRig, true);
            godState.SetActive(cameraRig, false);
            UpdateCamera(godState);
        }
        else
        {
            humanState.SetActive(cameraRig, false);
            godState.SetActive(cameraRig, true);
            godState.UpdateMetaphorPosition(cameraRig);

            UpdateCamera(humanState);
        }
    }

    class StateData
    {
        public GameObject positionObject;
        public bool active;

        public StateData (GameObject positionObject, bool active)
        {
            this.positionObject = positionObject;
            this.active = active;
        }

        public void UpdateMetaphorPosition(GameObject cameraRig)
        {
            positionObject.transform.position = cameraRig.transform.position;
            positionObject.transform.rotation = cameraRig.transform.rotation;
        }

        public void SetActive(GameObject cameraRig, bool active)
        {
            if (!active)
            {
                UpdateMetaphorPosition(cameraRig);
            }
            positionObject.SetActive(!active);
        }
    }
}
