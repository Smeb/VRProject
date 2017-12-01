using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandUIController : MonoBehaviour {
    private GameObject scanStatusMessage;
    private PlayerController playerController;
    private ShoppingListItemCollection shoppingListPanel;
    private bool scannerToggledOn;
    [SerializeField] private Button scanMode;

    private GameObject scanner;
    private Text scanModeText;
    private bool isVisible;
    private bool scannerOnline;
    private bool previousActiveState = true;
    private PlayerContainer[] containers;
    public HashSet<GameObject> containedItems = new HashSet<GameObject>();
    public HashSet<int> addedItems = new HashSet<int>();

    public event ToggleEvent ToggleShoppingList;
    public event ToggleEvent ToggleScanMode;

    void Awake ()
    {
        shoppingListPanel = GameObject.FindObjectOfType<ShoppingListItemCollection>();
        scanMode = GameObject.Find("ScanButton").GetComponent<Button>();
        scanModeText = scanMode.GetComponentInChildren<Text>();
        playerController = GetComponentInParent<PlayerController>();
        containers = GetComponentsInChildren<PlayerContainer>();
        UpdatePlayerContainerVisibilities();
        scanner = transform.Find("Scanner").gameObject;
        scanStatusMessage = scanner.transform.Find("ScanStatusMessage").gameObject;
        foreach (PlayerContainer container in containers)
        {
            container.AddItem += OnContainerAddItem;
            container.RemoveItem += OnContainerRemoveItem;
        }
    }

    void OnContainerAddItem(GameObject gameObject)
    {
        containedItems.Add(gameObject);
    }

    void OnContainerRemoveItem(GameObject gameObject)
    {
        containedItems.Remove(gameObject);
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

    private void ToggleCameraScanner(bool state)
    {
        scannerOnline = state;
        if (scannerOnline)
        {
            scanStatusMessage.transform.Find("Title").GetComponent<Text>().text = "Scanner Online";
            scanStatusMessage.transform.Find("Body").GetComponent<Text>().text = "";
        }
        else
        {
            scanStatusMessage.transform.Find("Title").GetComponent<Text>().text = "Scanner Offline";
            scanStatusMessage.transform.Find("Body").GetComponent<Text>().text = "Add items to enable scanner";
        }
    }

    public void AddItem(GameObject item)
    {
        addedItems.Add(item.GetComponent<ProductCode>().Code);
        ToggleCameraScanner(true);
        shoppingListPanel.AddItem(item);
    }

    public void ScanModeToggle()
    {
        if (scannerToggledOn)
        {
            if (ToggleScanMode != null)
            {
                ToggleScanMode(false);
            }
            scanModeText.text = "Enable Add Item Mode";
        }
        else
        {
            if (ToggleScanMode != null)
            {
                ToggleScanMode(true);
            }
            scanModeText.text = "Disable Add Item Mode";
        }
        scannerToggledOn = !scannerToggledOn;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        playerController.AddItem += AddItem;
        playerController.OnChangeState += OnChangeState;
        shoppingListPanel.gameObject.SetActive(true);
        shoppingListPanel.Initialise();
        scanStatusMessage.SetActive(true);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        playerController.AddItem -= AddItem;
        playerController.OnChangeState -= OnChangeState;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        ToggleCameraScanner(false);
        shoppingListPanel.gameObject.SetActive(true);
        shoppingListPanel.Initialise();
    }

    void UpdatePlayerContainerVisibilities()
    {
        if (playerController.activeState is HumanState)
        {
            foreach (PlayerContainer container in containers)
            {
                container.ToggleVisibility(isVisible);
            }
        } 
    }

    void ToggleVisibility()
    {
        if (isVisible != previousActiveState)
        {
            if (ToggleShoppingList != null)
            {
                ToggleShoppingList(isVisible);
            }

            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = isVisible;
            }

            UpdatePlayerContainerVisibilities();

            scanner.SetActive(isVisible);
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
