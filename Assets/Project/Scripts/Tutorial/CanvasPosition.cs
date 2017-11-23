using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPosition : MonoBehaviour {

	public MeshFilter host;
	public RectTransform rectTransform;
	GameObject dialogueBox;
	// Use this for initialization
	void Start () {
		host = GameObject.Find ("Cowboy").GetComponentInChildren<MeshFilter>() as MeshFilter;
		dialogueBox = GameObject.Find ("DialogueBox");
		rectTransform = dialogueBox.GetComponent<RectTransform> ();
	}

	// Update is called once per frame
	void Update () {
		//this is your object that you want to have the UI element hovering over
		//RectTransform CanvasRect=Canvas.GetComponent<RectTransform>();
		RectTransform CanvasRect= this.GetComponent<RectTransform> ();

	
		//then you calculate the position of the UI element
		//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
		Vector2 ViewportPosition=Camera.main.WorldToViewportPoint(host.transform.position);
		Vector2 WorldObject_ScreenPosition=new Vector2(
			((ViewportPosition.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
			((ViewportPosition.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)));


		//now you can set the position of the ui element
		rectTransform.anchoredPosition =WorldObject_ScreenPosition;

		// Check to see if we need to display the box based on the camera direction
		toggleVisibilityBasedOnAngle (); 
	}


	// Hide canvas if the user isn't facing the host
	void toggleVisibilityBasedOnAngle(){
		Vector3 heading = host.transform.position - Camera.main.transform.position; 

		float angle = Vector3.Dot(heading, Camera.main.transform.forward);
		//print (angle);

		if (angle < 0.0) {
			dialogueBox.SetActive (false);
		} else {
			dialogueBox.SetActive (true);
		}
	}
}
