using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {


	//Dialogue Parent 
	public GameObject canvasUI;

	//Dialogue text
	public Text canvasText;

	// Button
	public Button nextButton;
	public Button prevButton;


	public int instructionIndex; 


	//Host
	public GameObject host;



	//Camera State
	//public CameraSate cameraState;

	public AvatarAnimator avatarAnimator;

	public bool isPicked = false;
	public bool isInserted = false;

	// States
	public enum TutorialState {
		stepOne, // Run on spot
		stepTwo, // teleportation
		stepThree, // scanning items into shopping list
		stepFour, // put items into inventory using predefined shopping list 
		stepFive, // emptying inventory into final till
		stepSix, 
		stepSeven,
		stepEight
	}

	public TutorialState tutorialState;


	public Dictionary<TutorialState, string[]> dialogues;

	// Use this for initialization
	void Start () {
		tutorialState = TutorialState.stepOne;
		instructionIndex = 0; 

		nextButton = GameObject.Find("NextButton").GetComponent<Button>();
		prevButton = GameObject.Find("PrevButton").GetComponent<Button>();

		nextButton.onClick.AddListener(() => Next());
		prevButton.onClick.AddListener(() => Previous());

		dialogues = new Dictionary<TutorialState, string[]> ();

		populateDialogues ();


		UpdateText (); 




	}


	
	// Update is called once per frame
	void Update () {

		checkTutorialState ();
	}

	void checkTutorialState(){

		switch (this.tutorialState) {

		case TutorialState.stepOne:


			if (Vector3.Distance (this.transform.position, host.transform.position) < 3) {
				//canvasText.text = "Nice! Press the menu button to move to switch in and out of god mode";
				UpdateState(); 
				UpdateText (); 
			}
			break;

		case TutorialState.stepTwo:


			UpdateText (); 
		
			/*
			if (cameraState.GetType () == typeof(GodState)) {

				// scale up host

			}

			
			*/

			break;

		case TutorialState.stepThree:

			if (isPicked) {
				tutorialState++;
			}

			break;

		case TutorialState.stepFour:

			if (isInserted) {
				tutorialState++;
			}

			break;

		default: 
			break;
	

		}
	}
		

	void Next(){
		if (!nextButton.interactable) return;

		instructionIndex += 1;
		prevButton.interactable = true;

		if (instructionIndex == dialogues[tutorialState].Length - 1)
		{
			nextButton.interactable = false;
			//continueButton.interactable = true;
		}
		UpdateText();
	}

	void Previous(){
		if (!prevButton.interactable) return;
		instructionIndex -= 1;
		nextButton.interactable = true;
		if (instructionIndex == 0)
		{
			prevButton.interactable = false;
		}
		UpdateText();
	}

	public void UpdateText()
	{
		print (instructionIndex);
		canvasText.text = dialogues[tutorialState][instructionIndex];
	}


	public void UpdateState(){
		tutorialState++;
		instructionIndex = 0; 
	}

	void populateDialogues(){

		string[] stepOneInstructions = new string[2];

		// Step One
		stepOneInstructions[0] = "Run on the spot to move towards me. Simple. Make sure your head bobs up and down.";
		stepOneInstructions [1] = "Use your laser pointer to determine the direction";
		dialogues.Add (TutorialState.stepOne, stepOneInstructions);


		string[] stepTwoInstructions = new string[2];

		// Step Two
		stepTwoInstructions[0] = "Now teleport to NPC \n 1) Press the menu button, you will see an overview of the environment. ";
		stepTwoInstructions [1] = "2) Your position is indicated by a statue. Grab and drag it to your target location. \n3) Press the menu button again to be teleported.";
		dialogues.Add (TutorialState.stepTwo, stepTwoInstructions);


		// Step Three
		string[] stepThreeInstructions = new string[3];
		stepThreeInstructions [0] = "Tilt the palm of your left hand to face you. This is your shopping list.";
		stepThreeInstructions [1] = "Use the laser pointer to scan items into the shopping list. Point the laser on a target item and click to scan.";
		stepThreeInstructions [2] = "Now scan 3 items.";
		dialogues.Add (TutorialState.stepThree, stepThreeInstructions);


		// Step Four
		string[] stepFourInstructions = new string[3];
		stepFourInstructions [0] = "Time for a scavenger hunt! You need to find items in your updated shopping list and place them in your inventory.";
		stepFourInstructions [1] = "Items can be placed in your inventory by dragging them into the translucent spheres to your left and right.";
		stepFourInstructions [2] = "Now search for the six items in your shopping list.";
		dialogues.Add (TutorialState.stepFour, stepFourInstructions);

		// Step Five 
		string[] stepFiveInstructions = new string[1];
		stepFiveInstructions [0] = "Great job! Now go to the checkout till and take items from your inventory and place them on the checkout till.";
		dialogues.Add (TutorialState.stepFive, stepFiveInstructions);


	}
}
