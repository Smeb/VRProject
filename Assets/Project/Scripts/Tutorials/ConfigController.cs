using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigController : MonoBehaviour
{
    private PlayerController playerController;
    int instructionIndex;
    string[] instructions;

    private Button optionalButton, nextButton, prevButton, continueButton;
    private Text title, body, info, optionalButtonText;
    
    enum State { HEIGHT_CALIBRATION, BEFORE_CONTINUING, END_SCENE }
    [SerializeField] State activeState;

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
        optionalButtonText = optionalButton.GetComponentInChildren<Text>();

        continueButton.onClick.AddListener(() => Continue());
        nextButton.onClick.AddListener(() => Next());
        prevButton.onClick.AddListener(() => Previous());

        SetNextState();
    }
	
	public void SetHeight()
    {
        float height = playerController.SetHeight();
        info.text = "Calibrated height: " + height.ToString("F2") + "m";
        optionalButtonText.text = "Re-calibrate";
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

    private void EndScene()
    {
        GameObject.Find("SceneManager").GetComponent<PersistentSceneManager>().LoadLevel("Tutorial");
    }

    private void BeforeContinueSetup()
    {
        title.text = "Before you continue";
        instructions = new string[1];
        instructions[0] = "We're now going to load a test environment, to teach you to move and interact";
        info.text = "";

        optionalButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
        continueButton.enabled = true;
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
            case State.BEFORE_CONTINUING: BeforeContinueSetup();  break;
            case State.END_SCENE: EndScene(); break;
        }

        if (instructions.Length > 1)
        {
            continueButton.interactable = false;
            nextButton.interactable = true;
        }
        UpdateText();
    }
}
