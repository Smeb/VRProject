using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleVisibility : MonoBehaviour {

	// Use this for initialization


	public GameObject cube;

	public GameObject targetCube;

	public GameObject shoppingListPanel; 

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// mock some kind of rotation
		cube.transform.RotateAround ( cube.transform.position , new Vector3 (0, 1, 0), 5);
		//Debug.DrawLine (Camera.main.transform.forward, targetCube.transform.position);

		Vector3 direction = targetCube.transform.position - Camera.main.transform.forward; 

//		RaycastHit hitInfo;
//



//		Debug.DrawRay (Camera.main.transform.forward, direction, Color.blue); 
//
//		if( Physics.Raycast (Camera.main.transform.forward, direction, out hitInfo, 5)){
//			if (hitInfo.collider.gameObject.name == "VisibilityTarget") {
//				//print (hitInfo.distance); 
//				//print ("Need to activate panel");
//				shoppingListPanel.SetActive (true);
//			} else {
//				shoppingListPanel.SetActive (false); 
//			}
//		} else {
//			//print ("Need to hide panel"); 
//			shoppingListPanel.SetActive (false); 
//		}

	

	
		float angle = Vector3.Dot (targetCube.transform.forward, Camera.main.transform.forward);

		print (angle);

		if (angle < 0.0) {
			shoppingListPanel.SetActive (true);
		} else {
			shoppingListPanel.SetActive (false); 
		}



	}
}
