using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WandController : Owner
{
    private void HumanUpdate ()
    {
        if (controller.GetPressUp(triggerButton))
        {
            if (ownedItem)
            {
                if (container && !container.OwnsItem())
                {
                    GiveItem(container);
                }
                else
                {
                    ThrowObject();
                }
            }
        }

        if (controller.GetPressUp(toggleViewpoint))
        {
            playerController.ToggleViewpoint();
        }
    }

    public void HumanStateChangeHandler()
    {
        if (ownedItem)
        {
            ThrowObject();
        }
    }
}
