using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingListItem : MonoBehaviour {
	public bool ticked;

    private Sprite background;
    private Image imageBox;
    private Image tickImg;

	void Start()
    {
		ticked = false;
        imageBox = transform.Find("ImagePlaceHolder").GetComponent<Image>();
        tickImg = transform.Find("Tick").GetComponent<Image>();
    }

    public Color UpdateColorAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        return color;
    }

    public void ClearItem()
    {
        imageBox.sprite = background;
    }

	public void AddItem(Sprite iconSprite){
        background = imageBox.sprite;
		imageBox.sprite = iconSprite;
        imageBox.color = UpdateColorAlpha(imageBox, 1);
	}

    public void UntickBox()
    {
        tickImg.color = UpdateColorAlpha(tickImg, 0);
    }
	public void TickBox()
    {
        tickImg.color = UpdateColorAlpha(tickImg, 1);
    }
}
