using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : Owner {
    public float highlightAlpha = 0.35f;
    private Color originalColor;
    private Color highlightColor;
    protected FixedJoint fixedJoint;
    private float maxInnerRadius;
    private float originalScale;

    public override void Start()
    {
        base.Start();
        fixedJoint = gameObject.GetComponent<FixedJoint>();
        maxInnerRadius = GetComponent<Renderer>().bounds.size.x * 0.8f;
        originalColor = gameObject.GetComponent<Renderer>().material.color;
        highlightColor = new Color(originalColor.r, originalColor.g, originalColor.b, highlightAlpha);
    }

    private float FindLargestAxis(Property item)
    {
        Vector3 itemSize = item.GetComponent<Renderer>().bounds.size;
        return Mathf.Max(Mathf.Max(itemSize.x, itemSize.y), itemSize.z);
    }

    public float FindItemScale(Property item)
    {
        float largestAxisOfItem = FindLargestAxis(item);
        return (largestAxisOfItem > maxInnerRadius) ? maxInnerRadius / largestAxisOfItem : 1.0f;
    }

    protected override void TakeOwnership(Property item)
    { 
        base.TakeOwnership(item);
        float scale = FindItemScale(ownedItem);
        anchor.transform.localScale = new Vector3(scale, scale, scale);
        item.transform.rotation = transform.rotation;
        Vector3 offset = item.GetComponent<Renderer>().bounds.center - item.transform.position;
        item.transform.position = transform.position - offset;

        fixedJoint.connectedBody = item.GetComponent<Rigidbody>();
    }

    public override void GiveUpObject(Property item)
    {
        fixedJoint.connectedBody = null;
        base.GiveUpObject(item);
    }
}
