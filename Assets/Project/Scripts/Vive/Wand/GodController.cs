using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WandController : Owner
{
    private void GodUpdate()
    {
        if (controller.GetPressUp(triggerButton))
        {
            if (ownedItem)
            {
                ThrowObject();
            }
        }


        if (controller.GetPressUp(toggleViewpoint))
        {
            if (timer.CheckTimer(toggleViewpoint, 1))
            {
                GameObject humanPosition = playerController.GetHumanPosition();
                Rigidbody rigidbody = humanPosition.GetComponent<Rigidbody>();

                humanPosition.transform.position = gameObject.GetComponentInParent<SphereCollider>().transform.position;
                humanPosition.transform.rotation = new Quaternion(0, 0, 0, 0);

                rigidbody.velocity = new Vector3(0, Physics.gravity.y / 2, 0);
                rigidbody.angularVelocity *= 0;
            }
            else
            {
                playerController.ToggleViewpoint();
            }
        }
    }

    public void GodStateChangeHandler()
    {
        if(ownedItem)
        {
            ThrowObject();
        }
    }
}
