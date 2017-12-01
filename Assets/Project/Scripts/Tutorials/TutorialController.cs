using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{


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
    public GameObject target1; //After Step 1
    public PlayerContainer[] containers;
    HashSet<int> itemsToGet;

    public PlayerController playerController;

    // Bool to check if the user has entered god mod atleast one
    public bool hasBeenGod = false;
    public bool listenerSet = false;

    private HandUIController handController;
    private ShoppingListItemCollection shoppingListController;


    // States
    public enum TutorialState
    {
        stepOne, // Run on spot
        stepTwo, // teleportation
        stepThree, // scanning items into shopping list
        stepFour, // put items into inventory using predefined shopping list 
        stepFive, // emptying inventory into final till 
        stepSix
    }

    public TutorialState tutorialState;

    bool HasItemsInInventory()
    {
        if (handController.containedItems.Count < 3)
        {
            return false;
        }

        foreach (GameObject item in handController.containedItems)
        {
            if (!itemsToGet.Contains(item.GetComponent<ProductCode>().Code))
            {
                return false;
            }
        }
        return true;
    }

    public Dictionary<TutorialState, string[]> dialogues;

    // Use this for initialization
    void Start()
    {
        itemsToGet = new HashSet<int>();
        itemsToGet.Add(49);
        itemsToGet.Add(51);
        itemsToGet.Add(56);

        tutorialState = TutorialState.stepOne;
        instructionIndex = 0;
        containers = GameObject.FindObjectsOfType<PlayerContainer>();

        playerController = GetComponent<PlayerController>();

        host = GameObject.Find("eve_j_gonzales");
        animator = host.GetComponent<Animator>();

        canvasUI = GameObject.Find("TutorialPanel");
        canvasText = canvasUI.GetComponentInChildren<Text>();
        nextButton = GameObject.Find("NextButton").GetComponent<Button>();
        prevButton = GameObject.Find("PrevButton").GetComponent<Button>();

        nextButton.onClick.AddListener(() => Next());
        prevButton.onClick.AddListener(() => Previous());

        handController = GameObject.FindObjectOfType<HandUIController>();

        prevButton.interactable = false;
        nextButton.interactable = true;

        animator = host.GetComponent<Animator>();

        dialogues = new Dictionary<TutorialState, string[]>();
        populateDialogues();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {

        checkTutorialState();
    }

    void checkTutorialState()
    {

        switch (this.tutorialState)
        {

            case TutorialState.stepOne:
                if (Vector3.Distance(this.transform.position, host.transform.position) < 3)
                {
                    UpdateState();
                    UpdateText();


                }
                break;

            case TutorialState.stepTwo:
                if (playerController.activeState is GodState)
                {
                    hasBeenGod = true;
                    // scale up host
                    int runOnSpot = Animator.StringToHash("RunOnSpot");
                    animator.SetBool(runOnSpot, true);
                    MoveHostToTarget(target1);
                }
                if (hasBeenGod && Vector3.Distance(this.transform.position, host.transform.position) < 3)
                {
                    UpdateState();
                    UpdateText();
                }

                break;

            case TutorialState.stepThree:
                if (handController.addedItems.Count == 3)
                {
                    UpdateState();
                    UpdateText();
                }
                break;

            case TutorialState.stepFour:
                if (HasItemsInInventory())
                {
                    UpdateState();
                    UpdateText();
                }
                break;

            case TutorialState.stepFive:
                if (handController.containedItems.Count == 0)
                {
                    UpdateState();
                    UpdateText();
                }
                break;

            case TutorialState.stepSix:
                if (!listenerSet)
                {
                    listenerSet = true;
                    nextButton.onClick.RemoveAllListeners();
                    nextButton.onClick.AddListener(() => GameObject.Find("SceneManager").GetComponent<PersistentSceneManager>().LoadLevel("Supermarket_01"));
                    nextButton.GetComponent<Text>().text = "Continue";
                    Destroy(prevButton);
                }
                break;
        }
    }

    void Next()
    {
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

    void Previous()
    {
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
        print(instructionIndex);
        canvasText.text = dialogues[tutorialState][instructionIndex];
    }

    public void ResetButtonState()
    {
        prevButton.interactable = false;
        nextButton.interactable = true;
    }

    public void UpdateState()
    {
        tutorialState++;
        instructionIndex = 0;

        ResetButtonState();
    }


    void populateDialogues()
    {

        string[] stepOneInstructions = new string[2];

        // Step One
        stepOneInstructions[0] = "Press the touchpad, then run on the spot to move towards me. Make sure your head bobs up and down.";
        stepOneInstructions[1] = "Use your controller to determine the direction";
        dialogues.Add(TutorialState.stepOne, stepOneInstructions);


        string[] stepTwoInstructions = new string[2];

        // Step Two
        /* 
		 * At the end of step one, npc runs to X position and turns around, and starts waving
		 * UI box stays at current position
		 */
        stepTwoInstructions[0] = "There's a faster way to move.\n This is done by:";
        stepTwoInstructions[1] = "1) Press the menu button, you will see an overview of the environment. \n 2) Your position is indicated by a statue. Grab and drag it to your where you want to go. \n3) Press the menu button again to be swap places with the statue.";
        dialogues.Add(TutorialState.stepTwo, stepTwoInstructions);


        // Step Three

        // NPC doesn't do anything, UI moves to host
        string[] stepThreeInstructions = new string[4];
        stepThreeInstructions[0] = "Tilt the palm of your left hand to face you. This is your shopping list, inventory, and scanner.";
        stepThreeInstructions[1] = "To use the scanner you need to add some items first. Click \"Enabled Add Item mode\", then add the items on the table";
        stepThreeInstructions[2] = "Point the laser on a target item and press the trigger to add it.  Click \"Disable Add Item Mode\" to go back to being able to pick things up.";
        stepThreeInstructions[3] = "Now add 3 items.";
        dialogues.Add(TutorialState.stepThree, stepThreeInstructions);


        // Step Four
        // 
        string[] stepFourInstructions = new string[3];
        stepFourInstructions[0] = "Time for a scavenger hunt! You need to find items in your updated shopping list and place them in your inventory.";
        stepFourInstructions[1] = "Items can be placed in your inventory by dragging them into the spheres you see when you tilt your left hand.";
        stepFourInstructions[2] = "Now search for the items in your shopping list! You can use the scanner to highlight the objects in the environment to make them easier to find.";
        dialogues.Add(TutorialState.stepFour, stepFourInstructions);

        // Step Five 
        // Host to till , UI follows
        string[] stepFiveInstructions = new string[1];
        stepFiveInstructions[0] = "Great job! Now come to the checkout till and take items from your inventory and place them on the checkout till.";
        dialogues.Add(TutorialState.stepFive, stepFiveInstructions);

        string[] stepSixInstructions = new string[1];
        stepSixInstructions[0] = "That's everything scanned! We're now going to take you to the proper supermarket to do the actual task. Press Continue when you're ready!";
        dialogues.Add(TutorialState.stepSix, stepSixInstructions);
    }

    void MoveHostToTarget(GameObject target)
    {
        float step = 2.0f * Time.deltaTime;
        host.transform.position = Vector3.MoveTowards(host.transform.position, target.transform.position, step);
    }
}
