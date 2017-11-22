using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Owner : MonoBehaviour
{
    private Property m_ownedItem;
    protected Property ownedItem
    {
        get
        {
            return m_ownedItem;
        }
        set
        {
            if (value != null)
            {
                value.owner = this;
            }
            m_ownedItem = value;
        }
    }

    protected virtual void TakeOwnership(Property item)
    {
        ownedItem = item;
    }

    public virtual void GiveUpObject(Property item)
    {
        if (ownedItem == item)
        {
            ownedItem = null;
        }
    }

    public bool OwnsItem()
    {
        return ownedItem != null;
    }

    protected virtual void GiveItem(Owner otherOwner)
    {
        otherOwner.TakeOwnership(ownedItem);
    }

}
