using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelInit : MonoBehaviour
{
    // Use this for initialization
    public void Start()
    {
        TutorialController controller = GameObject.Find("[CameraRig]").AddComponent<TutorialController>();
    }
}