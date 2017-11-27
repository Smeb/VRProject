using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimator : MonoBehaviour {
    public float lastWave;
    Animator animationController;
	// Use this for initialization
	void Start () {
        lastWave = Time.time;
        animationController = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        animationController.SetFloat("TimeSinceWave", Time.time - lastWave);
	}
}
