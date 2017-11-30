using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {


	//Canvas elements 
	public GameObject canvasUI;
	public Text canvasText; 
	public Button nextButton;
	public Button prevButton;

	public int stateTest = 0;

	//Instruction Pointer
	public int instructionIndex; 

	//NPC Reference
	public GameObject host;

	public Animator animator;

	// Position References 
	public GameObject marker1; //After Step 1
	public GameObject marker2; //After Step 4


	public PlayerController playerController;
	public ShoppingListItemCollection shoppingListItemCollection; 

	// Bool to check if the user has entered god mod atleast one
	public bool hasBeenGod = false;
	public bool hasItemsInInventory = false;
	public bool allItemsScanned = false;

	public GameObject FPSController; // DELETE THIS
	// Animator states
	public int runOnSpot;
	public int turnRight;
	public int turnLeft;
	// States
	public enum TutorialState {
		stepOne, // Run on spot
		stepTwo, // teleportation
		stepThree, // scanning items into shopping list
		stepFour, // put items into inventory using predefined shopping list 
		stepFive // emptying inventory into final till
	}

	public TutorialState tutorialState;

	//public Container[] containers;


	public Dictionary<TutorialState, string[]> dialogues;

	// Use this for initialization
	void Start () {
		tutorialState = TutorialState.stepOne;
		instructionIndex = 0; 

        playerController = GetComponent<PlayerController>();
        shoppingListItemCollection = GetComponentInChildren<ShoppingListItemCollection>();
        host = GameObject.Find("eve_j_gonzales");
        animator = host.GetComponent<Animator>();

        canvasUI = GameObject.Find("TutorialPanel");
        canvasText = canvasUI.GetComponentInChildren<Text>();
		nextButton = GameObject.Find("NextButton").GetComponent<Button>();
		prevButton = GameObject.Find("PrevButton").GetComponent<Button>();

		nextButton.onClick.AddListener(() => Next());
		prevButton.onClick.AddListener(() => Previous());

		prevButton.interactable = false;
		nextButton.interactable = true; 

		marker1 = GameObject.Find ("Marker1");
		marker2 = GameObject.Find ("Marker2");

		animator = host.GetComponent<Animator> ();

		runOnSpot = Animator.StringToHash("Run");
		turnRight = Animator.StringToHash ("TurnRight");
		turnLeft = Animator.StringToHash ("TurnLeft");

		dialogues = new Dictionary<TutorialState, string[]> ();
		populateDialogues ();
		UpdateText ();

		FPSController = GameObject.Find ("FPSController");

		//containers = GameObject.Find ("Container");
	

	}
		
	// Update is called once per frame
	void Update () {

		checkTutorialState ();
	}

	void checkTutorialState(){

		switch (this.tutorialState) {

		case TutorialState.stepOne:
			if (Vector3.Distance (this.transform.position, host.transform.position) < 3) {
				UpdateState(); 
				UpdateText ();
			}
			break;

		case TutorialState.stepTwo:


			animator.SetBool(runOnSpot, true);
			MoveHostToTarget(marker1);


		/*
			if (playerController.activeState is GodState) {
				hasBeenGod = true;
				// scale up host
                int runOnSpot = Animator.StringToHash("RunOnSpot");
                animator.SetBool(runOnSpot, true);
                MoveHostToTarget(target1);
			}
		*/

			if (hasBeenGod && Vector3.Distance (this.transform.position, host.transform.position) < 3) {
				UpdateState (); 
				UpdateText (); 
			}
		
			break;

		case TutorialState.stepThree:

            if (shoppingListItemCollection.slots.Length == 3)
            {
				UpdateState(); 
				UpdateText (); 
			}
			break;

		case TutorialState.stepFour:
			
			if (hasItemsInInventory) {
				UpdateState(); 
				UpdateText ();

				// Now give the user items to pick up.

				shoppingListItemCollection.ClearAll ();
				shoppingListItemCollection.AddItem (55); 
				shoppingListItemCollection.AddItem (49);
				shoppingListItemCollection.AddItem (56);
				shoppingListItemCollection.AddItem (57);
			}
			break;

		case TutorialState.stepFive:
			if (allItemsScanned) {
				UpdateState (); 
				UpdateText (); 
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

	public void ResetButtonState()
	{
	prevButton.interactable = false;
	nextButton.interactable = true;
	}
	
	public void UpdateState(){
		tutorialState++;
		instructionIndex = 0;

        ResetButtonState(); 
	}


	void populateDialogues(){

		string[] stepOneInstructions = new string[2];

		// Step One
		stepOneInstructions[0] = "Run on the spot to move towards me. Simple. Make sure your head bobs up and down.";
		stepOneInstructions [1] = "Use your laser pointer to determine the direction";
		dialogues.Add (TutorialState.stepOne, stepOneInstructions);


		string[] stepTwoInstructions = new string[2];

		// Step Two
		/* 
		 * At the end of step one, npc runs to X position and turns around, and starts waving
		 * UI box stays at current position
		 */
		stepTwoInstructions[0] = "Now teleport to the NPC \n This is done by:";
		stepTwoInstructions [1] = "1) Press the menu button, you will see an overview of the environment. \n 2) Your position is indicated by a statue. Grab and drag it to your target location. \n3) Press the menu button again to be teleported.";
		dialogues.Add (TutorialState.stepTwo, stepTwoInstructions);


		// Step Three

		// NPC doesn't do anything, UI moves to host
		string[] stepThreeInstructions = new string[3];
		stepThreeInstructions [0] = "Tilt the palm of your left hand to face you. This is your shopping list and inventory.";
		stepThreeInstructions [1] = "Click \"Enable scanning mode\". \n Use the laser pointer to scan items into the shopping list. Point the laser on a target item and click to scan.  Click \"Disable Scanning Mode\".";
		stepThreeInstructions [2] = "Now scan 3 items.";
		dialogues.Add (TutorialState.stepThree, stepThreeInstructions);


		// Step Four
		// 
		string[] stepFourInstructions = new string[3];
		stepFourInstructions [0] = "Time for a scavenger hunt! You need to find items in your updated shopping list and place them in your inventory.";
		stepFourInstructions [1] = "Items can be placed in your inventory by dragging them into the translucent spheres when you tilt your left hand.";
		stepFourInstructions [2] = "Now search for the three items in your shopping list! Come back to me once you're done.";
		dialogues.Add (TutorialState.stepFour, stepFourInstructions);

		// Step Five 
		// Host to till , UI follows
		string[] stepFiveInstructions = new string[1];
		stepFiveInstructions [0] = "Great job! Now go to the checkout till and take items from your inventory and place them on the checkout till.";
		dialogues.Add (TutorialState.stepFive, stepFiveInstructions);


	}


	void MoveHostToTarget(GameObject target){
		float step = 2.0f * Time.deltaTime;

		// if you are close enough, look at the host!
		if (Vector3.Distance (target.transform.position, host.transform.position) <= 0.09) {
			animator.SetBool (runOnSpot, false);
			host.transform.LookAt (this.transform);

		} else {
			RotateTo (target, step);
			host.transform.position = Vector3.MoveTowards (host.transform.position, target.transform.position, step);
		}


	}


	void RotateTo(GameObject target, float step){
		Vector3 targetDir = target.transform.position - host.transform.position;
		Vector3 newDir = Vector3.RotateTowards (host.transform.forward, targetDir, step, 0.0F);
		//Debug.DrawRay (host.transform.position, newDir, Color.red);
		host.transform.rotation = Quaternion.LookRotation (newDir);
	}

}
