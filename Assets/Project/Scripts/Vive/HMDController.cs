using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMDController : MonoBehaviour {
    [SerializeField]
    private int touchpadIndex = -1;

    public void RegisterWand(WandController controller)
    {
        controller.OnTouchpadPress += TouchpadPressHandler;
        controller.OnTouchpadRelease += TouchpadReleaseHandler;
    }

    public void DeregisterWand(WandController controller)
    {
        controller.OnTouchpadPress -= TouchpadPressHandler;
        controller.OnTouchpadRelease -= TouchpadReleaseHandler;
    }

	private void TouchpadPressHandler(int index)
    {
        if (touchpadIndex == -1)
        {
            touchpadIndex = index;
            Debug.Log("Touchpad gained ownership of walking behaviour");
        }
        else
        {
            Debug.Log("Other touchpad is being used currently");
        }
    }

    private void TouchpadReleaseHandler(int index)
    {
        if (index == touchpadIndex)
        {
            touchpadIndex = -1;
        }
    }
}
