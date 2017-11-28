using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContainer : ContainerController {

    public void OnEnable()
    {
        PlayerController playerController = GameObject.FindObjectOfType<PlayerController>();
        if (playerController)
        {
            playerController.OnChangeState += OnChangeState;
        }
    }

    public void OnDisable()
    {
        PlayerController playerController = GameObject.FindObjectOfType<PlayerController>();
        if (playerController)
        {
            playerController.OnChangeState -= OnChangeState;
        }
    }

    private void OnChangeState(CameraState state)
    {
        if (state is HumanState)
        {
            ToggleVisibility(true);
        }
        else if (state is GodState)
        {
            ToggleVisibility(false);
        }
    }

    private void ToggleVisibility(bool toggle)
    {
        GetComponent<Renderer>().enabled = toggle;
        if (ownedItem)
        {
            ownedItem.GetComponent<Renderer>().enabled = toggle;
        }
    }

    public void ClearItem()
    {
        if (ownedItem != null)
        {
            GiveUpObject(ownedItem);
        }
    }
}
