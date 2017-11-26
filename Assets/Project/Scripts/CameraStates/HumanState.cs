using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanState : CameraState
{
    public override Vector3 position
    {
        get
        {
            Vector3 position = base.position;
            Vector3 cameraOffset = cameraRig.GetComponentInChildren<Camera>().transform.localPosition;
            cameraOffset.y = 0;
            return position - cameraOffset;
        }
    }

    public override void SetActive(bool active)
    {
        referenceObject.GetComponent<Renderer>().enabled = !active;
        base.SetActive(active);
    }

    public HumanState(GameObject cameraRig, GameObject referenceObject, GameObject referenceFloor, int scale, int forceScale)
        : base(cameraRig, referenceObject, referenceFloor, scale, forceScale)
    {
        if (referenceObject.GetComponent<PlayerPositionValidator>())
        {
            throw new UnityException("Missing expected component script on referenceObject");
        }
    }
}
