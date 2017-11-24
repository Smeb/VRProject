using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockInteraction : MonoBehaviour {

	// Use this for initialization
	public FPSPick draggingItem;

	public InventoryManager inventoryManager;


	void Start () {
		draggingItem = GameObject.Find ("FPSController").GetComponent<FPSPick> ();
		inventoryManager = GameObject.Find ("BGPanel").GetComponent<InventoryManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (draggingItem.dragging != null) {
			if (Input.GetKeyDown ("space")) {
				print ("space key was pressed");

				inventoryManager.addItem (draggingItem.dragging);



			}
		}	
	}
}
