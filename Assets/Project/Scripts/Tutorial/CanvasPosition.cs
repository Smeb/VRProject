using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPosition : MonoBehaviour {

	public MeshFilter host;
	public RectTransform dialogueBoxRectTransform;
	public RectTransform arrowImgTransform;

	public GameObject dialogueBox;
	public GameObject arrowImg;


	// We use these bools to keep track of the visibility state of the dialogue box and arrow img 
	public bool isDialogueBoxOn; 
	public bool isArrowImgOn;


	// Use this for initialization
	void Start () {
		host = GameObject.Find ("Cowboy").GetComponentInChildren<MeshFilter>() as MeshFilter;

		dialogueBox = GameObject.Find ("DialogueBox");

		dialogueBoxRectTransform = dialogueBox.GetComponent<RectTransform> ();

		arrowImg = GameObject.Find ("HostArrow"); //dialogueBox.GetComponentInChildren<Image> () as Image; 

		arrowImgTransform = arrowImg.GetComponent<RectTransform>();

		arrowImg.SetActive(false);

		isDialogueBoxOn = true; 
		isArrowImgOn = false;
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
		dialogueBoxRectTransform.anchoredPosition =WorldObject_ScreenPosition;

		arrowImgTransform.anchoredPosition = WorldObject_ScreenPosition;

		toggleBetweenDialogueAndArrow (); 


		toggleVisibilityBasedOnAngle (); 




	}


	// Hide canvas if the user isn't facing the host
	void toggleVisibilityBasedOnAngle(){
		Vector3 heading = host.transform.position - Camera.main.transform.position; 

		float angle = Vector3.Dot(heading, Camera.main.transform.forward);
		//print (angle);

		if (angle < 0.0) {
			if (isDialogueBoxOn) {
				dialogueBox.SetActive (false);
				isDialogueBoxOn = false; 
			}

			if (isArrowImgOn) {
				arrowImg.SetActive (false);
				isArrowImgOn = false;
			}

		} else {
			if (isDialogueBoxOn) {
				dialogueBox.SetActive (true);
				isDialogueBoxOn = true;
			}
			if (isArrowImgOn) {
				arrowImg.SetActive (true);
				isArrowImgOn = true;
			}
		}
	}


	//Toggle Between the dialogue box and the arrow depending on the distance between player and host 
	void toggleBetweenDialogueAndArrow(){

		float distance = Vector3.Distance (host.transform.position, Camera.main.transform.position);

		//print (distance); 
		if (distance > 10.0) {
			arrowImg.SetActive (true);
			isArrowImgOn = true; 

			dialogueBox.SetActive (false);
			isDialogueBoxOn = false; 
		} else {
			arrowImg.SetActive (false);
			isArrowImgOn = false; 

			dialogueBox.SetActive (true);
			isDialogueBoxOn = true; 
		}

	}
}
