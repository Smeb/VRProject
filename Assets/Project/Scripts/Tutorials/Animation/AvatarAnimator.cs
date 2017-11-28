using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimator : MonoBehaviour {
    public float lastWave;
    Animator animationController;
    GazeTracker gazeTracker;
    // Use this for initialization
    void Awake () {
        lastWave = -5;
        animationController = gameObject.GetComponent<Animator>();
        gazeTracker = GameObject.Find("[CameraRig]").GetComponentInChildren<GazeTracker>();
    }

    private void OnEnable()
    {
        gazeTracker.GazeEnter += OnGazeEnter;
        gazeTracker.GazeExit += OnGazeExit;
    }

    private void OnDisable()
    {
        gazeTracker.GazeEnter -= OnGazeEnter;
        gazeTracker.GazeExit -= OnGazeExit;
    }

    private void OnGazeEnter(GameObject target)
    {
        Debug.Log("Gaze Enter");
        if (target == this.gameObject)
        {
            animationController.SetBool("PlayerGaze", true);
        }
    }

    private void OnGazeExit(GameObject target)
    {
        if (target == this.gameObject)
        {
            animationController.SetBool("PlayerGaze", false);
        }
    }

    // Update is called once per frame
    void Update () {
        animationController.SetFloat("TimeSinceWave", Time.time - animationController.GetFloat("LastWave"));
	}
}
