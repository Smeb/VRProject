using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private PlayerController playerController;
    int instructionIndex;
    string[] instructions;

    private Button optionalButton, nextButton, prevButton, continueButton;
    private Text title, body, info, optionalButtonText;
    
    enum State { HEIGHT_CALIBRATION, MOVEMENT_TUTORIAL, STATE_SWAP_TUTORIAL, INVENTORY_TUTORIAL, COMBINED_TUTORIAL, BRIEFING }
    State activeState;

	// Use this for initialization
	void Start () {
        playerController = GameObject.Find("[CameraRig]").GetComponent<PlayerController>();
        activeState = State.HEIGHT_CALIBRATION;
        title = GameObject.Find("Title").GetComponent<Text>();
        body = GameObject.Find("Body").GetComponent<Text>();
        info = GameObject.Find("Info").GetComponent<Text>();

        continueButton = GameObject.Find("ContinueButton").GetComponent<Button>();
        nextButton = GameObject.Find("Next").GetComponent<Button>();
        prevButton = GameObject.Find("Previous").GetComponent<Button>();

        optionalButton = GameObject.Find("OptionalButton").GetComponent<Button>();
        optionalButtonText = optionalButton.GetComponent<Text>();

        continueButton.onClick.AddListener(() => Continue());
        nextButton.onClick.AddListener(() => Next());
        prevButton.onClick.AddListener(() => Previous());

        SetNextState();
    }
	
	public void SetHeight()
    {
        float height = playerController.SetHeight();
        info.text = "Calibrated height: " + height.ToString("F2") + "m";
        continueButton.interactable = true;
    }

    public void Continue()
    {
        if (!continueButton.interactable) return;
        activeState += 1;
        SetNextState();
    }

    public void UpdateText()
    {
        body.text = instructions[instructionIndex];
    }

    public void Next()
    {
        if (!nextButton.interactable) return;

        instructionIndex += 1;
        prevButton.interactable = true;

        if (instructionIndex == instructions.Length - 1)
        {
            nextButton.interactable = false;
            continueButton.interactable = true;
        }
        UpdateText();
    }

    public void Previous()
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

    private void MovementTutorialSetup()
    {
        title.text = "Movement Tutorial";
        instructions = new string[2];
        instructions[0] = "To move, press the touchpad button on one of your controllers, then move up and down on the spot";
        instructions[1] = "To complete the tutorial, move to the panel on your right";

        info.text = "";

        optionalButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(true);
    }

    private void HeightCalibrationSetup()
    {
        title.text = "Height Calibration";
        instructions = new string[1];
        instructions[0] = "Stand up straight, then press the button below to set your height";
        info.text = "Calibrated height: ";

        optionalButton.interactable = true;
        optionalButton.GetComponentInChildren<Text>().text = "Calibrate";
        continueButton.interactable = false;

        optionalButton.onClick.AddListener(() => SetHeight());
        nextButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);
    }

    private void SetNextState()
    {
        instructionIndex = 0;
        prevButton.interactable = false;

        switch (activeState)
        {
            case State.HEIGHT_CALIBRATION: HeightCalibrationSetup(); break;
            case State.MOVEMENT_TUTORIAL: MovementTutorialSetup();  break;
            case State.STATE_SWAP_TUTORIAL: break;
            case State.INVENTORY_TUTORIAL: break;
            case State.COMBINED_TUTORIAL: break;
            case State.BRIEFING: break;
        }

        if (instructions.Length > 1)
        {
            continueButton.interactable = false;
            nextButton.interactable = true;
        }
        UpdateText();
    }
}
