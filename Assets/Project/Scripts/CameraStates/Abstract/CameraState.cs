using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraState : Location
{
    public GameObject referenceObject;
    public GameObject referenceFloor;
    public GameObject cameraRig;

    public int scale;
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

    public CameraState(GameObject cameraRig, GameObject referenceObject, GameObject referenceFloor, int scale)
    {
        this.cameraRig = cameraRig;
        this.referenceObject = referenceObject;
        this.referenceFloor = referenceFloor;
        this.scale = scale;
    }

    public virtual void SetActive(bool active)
    {
        this.active = active;
    }

    public virtual void UpdateCamera()
    {
        SetCameraTransform(this.position, this.rotation, this.scale);
    }

    protected void SetCameraTransform(Vector3 position, Quaternion rotation, int scale)
    {
        cameraRig.transform.position = this.position;
        cameraRig.transform.localScale = new Vector3(1, 1, 1) * this.scale;
    }
}

public abstract class Location
{
    public abstract Vector3 position { get; }
    public abstract Quaternion rotation { get; }
}