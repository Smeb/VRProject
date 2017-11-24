using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject cameraRig;

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
                m_activeState.UpdateCamera(cameraRig);
            }
        }
    }
    private CameraState humanState, godState;

    void Start()
    {
        humanState = new HumanState(humanReferencePosition, supermarketFloor, new Vector3(1, 1, 1));
        godState = new GodState(godReferencePosition, sceneFloor, new Vector3(30, 30, 30));
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

    public class GodState : CameraState
    {
        public override void UpdateCamera(GameObject cameraRig)
        {
            SetCameraTransform(cameraRig, this.position, cameraRig.transform.rotation, this.scale);
        }

        public GodState(GameObject referenceObject, GameObject referenceFloor, Vector3 scale) : base(referenceObject, referenceFloor, scale) {}
    }

    public class HumanState : CameraState
    {
        public override void UpdateCamera(GameObject cameraRig)
        {
            Camera camera = cameraRig.GetComponent<Camera>();
            camera.transform.position = this.position;
            camera.transform.rotation = this.rotation;
            cameraRig.transform.localScale = this.scale;
        }

        public override void SetActive(bool active)
        {
            referenceObject.GetComponent<Renderer>().enabled = !active;
            base.SetActive(active);
        }

        public HumanState(GameObject referenceObject, GameObject referenceFloor, Vector3 scale) : base(referenceObject, referenceFloor, scale)
        {
            if (referenceObject.GetComponent<PlayerPositionValidator>())
            {
                throw new UnityException("Missing expected component script on referenceObject");
            }
        }
    }



    public abstract class CameraState : Location
    {
        public GameObject referenceObject;
        public GameObject referenceFloor;

        public Vector3 scale;
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

        public CameraState(GameObject referenceObject, GameObject referenceFloor, Vector3 scale)
        {
            this.referenceObject = referenceObject;
            this.referenceFloor = referenceFloor;
            this.scale = scale;
        }

        public virtual void SetActive(bool active)
        {
            this.active = active;
        }

        public virtual void UpdateCamera (GameObject cameraRig)
        {
            SetCameraTransform(cameraRig, this.position, this.rotation, this.scale);
        }

        protected void SetCameraTransform (GameObject cameraRig, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            cameraRig.transform.position = this.position;
            cameraRig.transform.rotation = this.rotation;
            cameraRig.transform.localScale = this.scale;
        }
    }

    public abstract class Location
    {
        public abstract Vector3 position { get; }
        public abstract Quaternion rotation { get; }
    }
}
