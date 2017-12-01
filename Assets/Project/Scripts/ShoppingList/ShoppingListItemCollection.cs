using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class ShoppingListItemCollection : MonoBehaviour {
	public Transform itemsParent;

	[SerializeField] public ShoppingListItem[] slots;
    private int freeSlotIndex = 0;
	public IconMap iconMap;
	public GameObject BGPanel;
    public Material markerMaterial;
	public bool active;
    private HashSet<GameObject> spawnedItemCopy = new HashSet<GameObject>();

    public UCL.COMPGV07.Experiment experiment;

	public ItemChecker itemChecker;

    void Awake()
    {
        slots = this.GetComponentsInChildren<ShoppingListItem>();
        iconMap = new IconMap();
    }
    
    public void Initialise()
    {
        GameObject experimentGameObject = GameObject.Find("Checkout");
        if (experimentGameObject)
        {
            experiment = experimentGameObject.GetComponent<UCL.COMPGV07.Experiment>();
        }
        
        this.ClearAll();
        if (experiment)
        {
            Debug.Log(experiment.ItemsToCollect.Length);
            itemChecker = new ItemChecker(experiment.ItemsToCollect);

            foreach(GameObject spawnedItem in experiment.spawnedItems)
            {
                spawnedItemCopy.Add(spawnedItem);
            }
        }
    }

    public void ClearAll()
    {
        experiment = null;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearItem();
        }
        freeSlotIndex = 0;
    }

    public void ClearLast()
    {
        if (freeSlotIndex > 0)
        {
            freeSlotIndex--;
            slots[freeSlotIndex].ClearItem();
        }
    }

    public int QueryAddedItems()
    {
        return freeSlotIndex;
    }

	public void AddItem(GameObject item){
        spawnedItemCopy.Remove(item);
        int code = item.GetComponent<ProductCode>().Code;

        foreach (GameObject spawnedItem in spawnedItemCopy)
        {
            if (spawnedItem.GetComponent<ProductCode>().Code == code)
            {
                GameObject marker = Instantiate(spawnedItem);
                Destroy(marker.GetComponent<Collider>());
                Destroy(marker.GetComponent<Rigidbody>());
                marker.transform.position = spawnedItem.transform.position;
                marker.transform.parent = spawnedItem.transform;
                marker.GetComponent<Renderer>().material = markerMaterial;
            }
        }

        if (freeSlotIndex < slots.Length)
        {
            Sprite spriteIcon = iconMap.GetIconPath(code);
            slots[freeSlotIndex].AddItem(spriteIcon);
            freeSlotIndex++;
        }
	}
}

public class ItemChecker{

	private Dictionary<int, int> itemCollection;

	public ItemChecker(int[] itemsToCollect) {
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

public class IconMap : MonoBehaviour  {


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


