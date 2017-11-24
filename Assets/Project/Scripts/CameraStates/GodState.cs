using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodState : CameraState
{
    public override void UpdateCamera()
    {
        SetCameraTransform(this.position, cameraRig.transform.rotation, this.scale);
    }

    public GodState(GameObject cameraRig, GameObject referenceObject, GameObject referenceFloor, int scale)
        : base(cameraRig, referenceObject, referenceFloor, scale) { }
}