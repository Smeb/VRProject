using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShoppingListController : MonoBehaviour {
    private PlayerController playerController;
    private SteamVR_Camera camera;
    private ShoppingListItemCollection shoppingListPanel;
    private bool scannerToggled;
    [SerializeField] private Button scanMode;
    private Text scanModeText;
    private bool isVisible;
    private bool previousActiveState = true;

    public event Action ShoppingListOpen, ShoppingListClose, ScanModeOn, ScanModeOff;

    void Awake ()
    {
        shoppingListPanel = GameObject.FindObjectOfType<ShoppingListItemCollection>();
        scanMode = GameObject.Find("ScanButton").GetComponent<Button>();
        scanModeText = scanMode.GetComponentInChildren<Text>();
        playerController = GetComponentInParent<PlayerController>();
    }

    public void AddItem(int code)
    {
        shoppingListPanel.AddItem(code);
    }

    public void ScanModeToggle()
    {
        if (scannerToggled)
        {
            if (ScanModeOff != null)
            {
                ScanModeOn();
            }
            scanModeText.text = "Disable Scan Mode";
        }
        else
        {
            if (ScanModeOn != null)
            {
                ScanModeOff();
            }
            scanModeText.text = "Enable Scan Mode";
        }
        scannerToggled = !scannerToggled;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        playerController.AddItem += AddItem;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        playerController.AddItem -= AddItem;
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
