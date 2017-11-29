// Pointer visual behaviour based on the SteamVR_LaserPointer script
// Pointer functionality changed to use the collided object, rather than the parent component

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void PointerEventHandler(object sender, Transform targetTransform);


public class LaserPointer : MonoBehaviour
{
    [SerializeField] public bool scanMode = false;
    public Color defaultColor;
    public Color scanOnColor;
    public Color scanOffColor;
    private MeshRenderer pointerMeshRenderer;

    private Material defaultMaterial;
    private Material scanOnMaterial;
    private Material scanOffMaterial;

    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    public bool addRigidBody = false;
    public Transform reference;
    public event PointerEventHandler PointerIn, PointerOut;
    private List<string> masks;

    Transform previousContact = null;

	// Use this for initialization
	void Start ()
    {
        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
		holder.transform.localRotation = Quaternion.identity;

		pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
		pointer.transform.localRotation = Quaternion.identity;
		BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if(collider)
            {
                Object.Destroy(collider);
            }
        }

        InitMaterial(out defaultMaterial, defaultColor);
        InitMaterial(out scanOnMaterial, scanOnColor);
        InitMaterial(out scanOffMaterial, scanOffColor);

        pointerMeshRenderer = pointer.GetComponent<MeshRenderer>();
        pointerMeshRenderer.material = defaultMaterial;
        masks = new List<string>();
        masks.Add("Interface");
	}

    public void InitMaterial(out Material material, Color color)
    {
        material = new Material(Shader.Find("Unlit/Color"));
        material.SetColor("_Color", color);
    }

    public void ScanModeToggle(bool newScanMode)
    {
        if (newScanMode == true)
        {
            masks.Add("Grabbable");
        }
        else
        {
            masks.Remove("Grabbable");
        }
        scanMode = newScanMode;
    }

    public virtual void OnPointerIn(Transform target)
    {
        if (PointerIn != null)
        {
            PointerIn(this, target);
        }
    }

    public virtual void OnPointerOut(Transform target)
    {
        if (PointerOut != null)
        {
            PointerOut(this, target);
        }
    }

	void Update ()
    {
        float dist = 100f;

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool bHit = Physics.Raycast(raycast, out hit, 100000f, LayerMask.GetMask(masks.ToArray()));
        pointer.GetComponent<Renderer>().enabled = (scanMode) ? scanMode : bHit;

        if (bHit)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
            {
                pointerMeshRenderer.material = scanOnMaterial;
            }
            else
            {
                pointerMeshRenderer.material = defaultMaterial;
            }

            dist = hit.distance;
            if (previousContact != hit.collider.transform)
            {
                OnPointerOut(previousContact);
                OnPointerIn(hit.collider.transform);
                previousContact = hit.collider.transform;
            }
        }
        else
        {
            if (scanMode == true)
            {
                pointerMeshRenderer.material = scanOffMaterial;
            }
            if (previousContact != null)
            {
                OnPointerOut(previousContact);
            }
            previousContact = null;
        }

        pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        pointer.transform.localPosition = new Vector3(0f, 0f, dist/2f);
    }
}
