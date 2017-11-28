using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShoppingListController : MonoBehaviour {
    private SteamVR_Camera camera;
    private ShoppingListItemCollection shoppingListPanel;
    private bool isVisible;
    private bool previousActiveState = true;

	void Awake ()
    {
        shoppingListPanel = GameObject.FindObjectOfType<ShoppingListItemCollection>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        shoppingListPanel.ClearAll();
    }

    void ToggleVisibility()
    {
        if (isVisible != previousActiveState)
        {
            shoppingListPanel.gameObject.SetActive(isVisible);
            previousActiveState = isVisible;
        }
    }

    // Update is called once per frame
    void Update () {
		float angle = Vector3.Dot (this.transform.forward, Camera.main.transform.forward);

        if (angle < -0.71 && !isVisible) {
            isVisible = true;
        } else if (angle > -0.69 && isVisible) {
            isVisible = false;
        }
        ToggleVisibility();
    }
}
