using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : Location
{
    public GameObject referenceObject;
    public GameObject referenceFloor;

    public int scale;
    public int forceScale;
    public bool active;

    public override Vector3 position
    {
        get
        {
            Vector3 position = new Vector3(0, referenceFloor.transform.position.y, 0);
            Vector3 referenceBounds = referenceObject.GetComponent<Renderer>().bounds.center;
            position.x = referenceBounds.x;
            position.z = referenceBounds.z;
            return position;
        }
    }

    public override Quaternion rotation
    {
        get { return referenceObject.transform.rotation; }
    }

    public CameraState(GameObject referenceObject, GameObject referenceFloor, int scale, int forceScale)
    {
        this.referenceObject = referenceObject;
        this.referenceFloor = referenceFloor;
        this.scale = scale;
        this.forceScale = forceScale;
    }

    public virtual void SetActive(bool active)
    {
        this.active = active;
    }
}

public abstract class Location
{
    public abstract Vector3 position { get; }
    public abstract Quaternion rotation { get; }
}
