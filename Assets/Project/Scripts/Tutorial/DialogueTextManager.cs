using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTextManager : MonoBehaviour {


	enum TutorialState { stepOne, stepTwo, finished };
	private TutorialState state; // = TutorialState.stepOne; 
	public Text dialogue; 
	public GameObject host; 
	public GameObject player;

	// Use this for initialization
	void Start () {
		state = TutorialState.stepOne;
		dialogue = GameObject.Find("DialogueParent").GetComponentInChildren (typeof(Text)) as Text;
		player = GameObject.Find ("FPSController");
		host = GameObject.Find ("Cowboy");
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {

		case TutorialState.stepOne:
			if (Vector3.Distance (player.transform.position, host.transform.position) < 3) {
				completeStepOne (); 
				state++;
			}
			break;
		
	    /*
		case TutorialState.stepTwo:
			if (draggingItem.dragging != null && Vector3.Distance (player.transform.position, robot.transform.position) < 3) {
				completeStepTwo ();
				state++;
			}
			break;

		case TutorialState.finished:
			finished ();
			break; 

        */
		}
		

	}

	void completeStepOne(){
		print ("Im close enough now!");
		print (dialogue);
		if (dialogue != null) {
			print (dialogue.text);
			dialogue.text = "Well done! Now pick up an item";
		}

	}


}
