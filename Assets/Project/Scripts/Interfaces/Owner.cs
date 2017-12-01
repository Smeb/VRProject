using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Owner : MonoBehaviour
{
    [SerializeField] private Property m_ownedItem;
    protected GameObject anchor;
    protected Transform itemPreviousParent;

    protected virtual Property ownedItem
    {
        get
        {
            return m_ownedItem;
        }
        set
        {
            if (m_ownedItem != value)
            {
                if (m_ownedItem)
                {
                    m_ownedItem.transform.parent = itemPreviousParent;
                    m_ownedItem.transform.localScale = new Vector3(1, 1, 1);
                }

                if (value != null)
                {
                    value.owner = this;
                    itemPreviousParent = value.transform.parent;
                    value.transform.parent = anchor.transform;
                }
                else
                {
                    itemPreviousParent = null;
                }

                m_ownedItem = value;
            }
        }
    }

    public virtual void Start()
    {
        anchor = new GameObject("Anchor");
        anchor.transform.parent = this.transform;
        anchor.transform.localPosition = GetComponent<SphereCollider>().center;
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
