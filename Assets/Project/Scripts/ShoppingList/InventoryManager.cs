using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryManager : MonoBehaviour {

	// Use this for initialization

	public Transform itemsParent;


	public InventorySlot[] slots;
	public int itemCount;
	public IconMap iconMap;
	public GameObject BGPanel;
	public bool active;

	public GameObject parentGO; 

	public ItemChecker itemChecker; 

	void Start () {
		slots = this.GetComponentsInChildren<InventorySlot> ();
		itemCount = 0;
		iconMap = new IconMap (); 

		//		BGPanel.SetActive (false);
		//		active = false;

		int[] itemsToCheck = { 55, 55, 55 };
		itemChecker = new ItemChecker (itemsToCheck);

		this.transform.position = parentGO.transform.position;


	}

	// Update is called once per frame
	void Update () {

		//
		//		if (Input.GetKeyDown (KeyCode.Tab)) {
		//			BGPanel.SetActive (true); 
		//		}
		//
		//		if (Input.GetKeyUp (KeyCode.Tab)) {
		//			BGPanel.SetActive (false); 
		//		}



	}

	public void addItem(GameObject newItem){
		// Get the product code from the item

		ProductCode code = newItem.GetComponent<ProductCode> () as ProductCode;
		print (code.Code);
		if (slots [itemCount].filled == false) {
			print ("free slot");
			Sprite spriteIcon = iconMap.GetIconPath (code.Code);
			slots [itemCount].AddItem (spriteIcon);

			if (itemChecker.isTickable (code.Code)) {
				print ("activating tick"); 
				slots [itemCount].activateTick (); 
			}


			itemCount++;



		} else {
			print ("ERROR, out of space in invent?"); 
		}

	}

}


public class ItemChecker{

	private Dictionary<int, int> itemCollection;

	public ItemChecker(int[] itemsToCollect){


		for (int i = 0; i < itemsToCollect.Length; i++){

			bool hasKey = itemCollection.ContainsKey (itemCollection [i]);

			int existingCount = hasKey ? itemCollection[itemsToCollect[i]] : 0; 

			itemCollection.Add (itemsToCollect [i], ++existingCount ); 
		}
			

	}

	public bool isTickable(int productCode){

		int existingValue = this.itemCollection [productCode];

		if (existingValue > 0) {
			itemCollection [productCode] = --existingValue;
			return true;
		} else {
			return false; 
		}

	}
		

}

public class IconMap :MonoBehaviour  {


	private Dictionary<int, Sprite> iconDict; 

	private string folderPath = "Assets/Project/Sprites/UniqueItemSprites/";

	public IconMap () {

		int[] productCodes = { 49, 50, 51, 52, 53, 54, 55, 56 };

		iconDict = new Dictionary<int, Sprite>();

		for (int i = 0; i < productCodes.Length; i++) {
			iconDict.Add (productCodes[i], LoadNewSprite(folderPath + productCodes[i] + ".png"));
		}

	}

	public Sprite GetIconPath(int productCode){

		//return this.iconDict [55 ];
		return this.iconDict [productCode]; 
	}



	//TAKEN FROM https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/

	public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f) {

		// Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

		Sprite NewSprite = new Sprite();
		Texture2D SpriteTexture = LoadTexture(FilePath);
		NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), PixelsPerUnit);

		return NewSprite;
	}

	public Texture2D LoadTexture(string FilePath) {

		// Load a PNG or JPG file from disk to a Texture2D
		// Returns null if load fails

		print (FilePath); 
		Texture2D Tex2D;
		byte[] FileData;

		if (File.Exists(FilePath)){
			FileData = File.ReadAllBytes(FilePath);
			Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
			if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
				return Tex2D;                 // If data = readable -> return texture
		}

		print ("ERROR CREATING SPRITE"); 
		return null;                     // Return null if load failed
	}


}


