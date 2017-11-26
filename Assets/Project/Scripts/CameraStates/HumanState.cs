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
            Vector3 cameraOffset = cameraRig.GetComponentInChildren<SteamVR_Camera>().head.localPosition;
            cameraOffset.y = 0;
            return position - cameraOffset;
        }
    }

    public override void SetActive(bool active)
    {
        Renderer renderer = referenceObject.GetComponent<Renderer>();
        renderer.enabled = !active;
        if (!active)
        {
            // Update the position of the reference object based on the user's current position
            Vector3 positionOriginOffset = referenceObject.transform.position - renderer.bounds.center;
            Vector3 newObjectPosition = cameraRig.GetComponentInChildren<SteamVR_Camera>().head.position + positionOriginOffset;
            newObjectPosition.y = referenceFloor.transform.position.y + renderer.bounds.size.y / 2;
            referenceObject.transform.position = newObjectPosition;
        }
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
