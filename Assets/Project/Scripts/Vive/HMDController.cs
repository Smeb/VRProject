using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMDController : MonoBehaviour {

    public delegate void StepTaken();
    public event StepTaken OnStepTaken;

    private SteamVR_Camera HMD;
    private GameObject cameraRig;
    private float earliestY;
    private float middleY;
    private float heightMin = 1.7f;
    private int steps;

    // Use this for initialization
    void Start () {
        cameraRig = gameObject.GetComponentInParent<SteamVR_PlayArea>().gameObject;
        HMD = GetComponent<SteamVR_Camera>();
    }

    private void FixedUpdate()
    {
        float latestY = HMD.head.position.y - cameraRig.transform.position.y;

        if (OnStepTaken != null)
        {
            if (latestY > heightMin)
            {
                if ((earliestY - middleY) + (latestY - middleY) > 0.0016f)
                {
                    bool minimum = IsMinimum(earliestY, middleY, latestY);
                    if (minimum)
                    {
                        OnStepTaken();
                    }
                }
            }
        }

        earliestY = middleY;
        middleY = latestY;
    }

    private bool IsMinimum(float earliest, float middle, float latest)
    {
        return earliest > middle && latest > middle;
    }
}
