using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInteractableInterface : MonoBehaviour {
    public event Action OnOver;         // Wand Raycast hits the object
    public event Action OnOut;          // Wand Raycast leaves the object
    public event Action OnClick;        // Wand trigger clicked over the object

    public void Over()
    {
        if (OnOver != null)
        {
            OnOver();
        }
    }
    
    public void Out()
    {
        if (OnOut != null)
        {
            OnOut();
        }
    }

    public void Click()
    {
        if (OnClick != null)
        {
            OnClick();
        }
    }
}
