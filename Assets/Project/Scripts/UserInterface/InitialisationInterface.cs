using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRInteractableInterface))]
public class InitialisationInterface : MonoBehaviour {
    [SerializeField] private VRInteractableInterface m_InteractiveItem;

    private void OnEnable()
    {
        m_InteractiveItem.OnClick += HandleOnClick;
        m_InteractiveItem.OnOver  += HandleOnOver;
        m_InteractiveItem.OnOut   += HandleOnOut;
    }

    private void OnDisable()
    {
        m_InteractiveItem.OnClick -= HandleOnClick;
        m_InteractiveItem.OnOver  -= HandleOnOver;
        m_InteractiveItem.OnOut   -= HandleOnOut;
    }

    private void HandleOnClick()
    {

    }

    private void HandleOnOver()
    {

    }

    private void HandleOnOut()
    {

    }
}
