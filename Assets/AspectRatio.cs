using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatio : MonoBehaviour {
    float targetAspect = 9f / 3f;
    private void Start()
    {
        GetComponent<Camera>().aspect = targetAspect;
    }
}
