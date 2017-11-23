using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	// Use this for initialization


	public bool filled;

	public Image imageBox;

	void start(){
		filled = false;

	}

	public void AddItem(Sprite iconSprite){

		imageBox.sprite = iconSprite;

		Color c = imageBox.color;
		c.a = 1; 

		imageBox.color = c; 

	}
}
