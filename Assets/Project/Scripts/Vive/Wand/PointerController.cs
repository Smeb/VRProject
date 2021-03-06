using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class WandController : Owner
{
    private Button button; // The selected button, if it exists
    private GameObject currentProduct;

    // Pointer specific scripts
    private LaserPointer pointer;
    private EventSystem eventSystem;
    private HandUIController handUIController;
    [SerializeField] private bool shoppingListOpen = false;
    private bool scanModeOn = false;

    private void OnShoppingListToggle(bool newState)
    {
        shoppingListOpen = newState;
    }

    private void OnScanModeToggle(bool state)
    {
        scanModeOn = state;
        pointer.ScanModeToggle(scanModeOn);
    }
    
    public void OnPointerEnter(object sender, Transform target)
    {
        Button buttonTarget = target.GetComponent<Button>();

        if (buttonTarget != null && buttonTarget.interactable)
        {
            buttonTarget.Select();
            button = buttonTarget;
            return;
        }

        ProductCode productTarget = target.GetComponent<ProductCode>();
        if (productTarget != null)
        {
            currentProduct = target.gameObject;
        }
    }

    public void OnPointerExit(object sender, Transform target)
    {
        if (button != null)
        {
            eventSystem.SetSelectedGameObject(null);
            button = null;
        }
        currentProduct = null;
    }

    void CheckForPointerInteractables()
    {
        if (pointer)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
    }
}
