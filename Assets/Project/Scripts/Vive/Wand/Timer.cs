using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {
    private Dictionary<Valve.VR.EVRButtonId, float> timers;

    public Timer()
    {
        timers = new Dictionary<Valve.VR.EVRButtonId, float>();
    }

    public void StartTimer (Valve.VR.EVRButtonId id)
    {
        timers[id] = Time.time;
    }

    public bool CheckTimer (Valve.VR.EVRButtonId id, int seconds)
    {
        if (Time.time - timers[id] > seconds)
        {
            return true;
        }
        return false;
    }
}
