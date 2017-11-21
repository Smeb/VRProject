using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : MonoBehaviour {
    private Owner m_owner;
    public Owner owner {
        set
        {
            if (m_owner != null && m_owner != value)
            {
                m_owner.GiveUpObject(this);
            }
            m_owner = value;
        }

        get
        {
            return m_owner;
        }
    }
}
