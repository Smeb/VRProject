using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandUIController : MonoBehaviour {
    private PlayerController playerController;
    private ShoppingListItemCollection shoppingListPanel;
    private bool scannerToggledOn;
    [SerializeField] private Button scanMode;
    private Text scanModeText;
    private bool isVisible;
    private bool previousActiveState = true;
    private PlayerContainer[] containers;

    public event Action ShoppingListOpen, ShoppingListClose, ScanModeOn, ScanModeOff;

    void Awake ()
    {
        shoppingListPanel = GameObject.FindObjectOfType<ShoppingListItemCollection>();
        scanMode = GameObject.Find("ScanButton").GetComponent<Button>();
        scanModeText = scanMode.GetComponentInChildren<Text>();
        playerController = GetComponentInParent<PlayerController>();
        containers = GetComponentsInChildren<PlayerContainer>();
    }

    private void OnChangeState(CameraState state)
    {
        if (state is GodState)
        {
            foreach(PlayerContainer container in containers)
            {
                container.ToggleVisibility(false);
            }
        }
        if (state is HumanState && isVisible)
        {
            foreach (PlayerContainer container in containers)
            {
                container.ToggleVisibility(true);
            }
        }
    }

    public void AddItem(int code)
    {
        shoppingListPanel.AddItem(code);
    }

    public void ScanModeToggle()
    {
        if (scannerToggledOn)
        {
            if (ScanModeOff != null)
            {
                ScanModeOff();
            }
            scanModeText.text = "Enable Scan Mode";
        }
        else
        {
            if (ScanModeOn != null)
            {
                ScanModeOn();
            }
            scanModeText.text = "Disable Scan Mode";
        }
        scannerToggledOn = !scannerToggledOn;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        playerController.AddItem += AddItem;
        playerController.OnChangeState += OnChangeState;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        playerController.AddItem -= AddItem;
        playerController.OnChangeState -= OnChangeState;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        shoppingListPanel.ClearAll();
    }

    void ToggleVisibility()
    {
        if (isVisible != previousActiveState)
        {
            if (isVisible && ShoppingListOpen != null)
            {
                ShoppingListOpen();
            }
            else if (!isVisible && ShoppingListClose != null)
            {
                ShoppingListClose();
            }

            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = isVisible;
            }

            if (playerController.activeState is HumanState)
            {
                foreach (PlayerContainer container in containers)
                {

                    container.ToggleVisibility(isVisible);
                }
            }
            
            shoppingListPanel.gameObject.SetActive(isVisible);
            previousActiveState = isVisible;
        }
    }

    // Update is called once per frame
    void Update () {
		float angle = Vector3.Dot (this.transform.forward, Camera.main.transform.forward);

        if (angle > 0.71 && !isVisible) {
            isVisible = true;
        } else if (angle < 0.69 && isVisible) {
            isVisible = false;
        }
        ToggleVisibility();
    }
}
