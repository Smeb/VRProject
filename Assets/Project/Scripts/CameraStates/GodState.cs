using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodState : CameraState
{
    public GodState(GameObject referencePosition, GameObject referenceFloor, int scale, int forceScale)
        : base(referencePosition, referenceFloor, scale, forceScale)
    {}
}
