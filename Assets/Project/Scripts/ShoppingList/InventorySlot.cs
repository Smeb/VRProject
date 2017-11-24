using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	// Use this for initialization


	public bool filled;

	public bool ticked;

	public Image imageBox;
	public Image tickImg;

	void start(){
		filled = false;
		ticked = false;

	}

	public void AddItem(Sprite iconSprite){

		imageBox.sprite = iconSprite;

		Color c = imageBox.color;
		c.a = 1; 

		imageBox.color = c; 

	}

	public void activateTick(){
		
		Color c = tickImg.color;
		c.a = 1;
		tickImg.color = c;
	}
}
