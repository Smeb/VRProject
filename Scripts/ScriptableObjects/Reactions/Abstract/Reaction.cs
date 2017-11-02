using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reaction : ScriptableObject {

    // Use this for initialization
    public void Init()
    {
        InitOverride();
    }

    // Can be overriden by implementing class
    protected virtual void InitOverride()
    { }
    
    public void React(MonoBehaviour monoBehaviour)
    {
        ImmediateReaction();
    }

    // Must be overriden by implementing class
    protected abstract void ImmediateReaction();
}
