using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
    public AudioSource source;
    public AudioClip hover;
    public AudioClip click;

    public void onHover()
    {
        source.PlayOneShot(hover);
    }

    public void onClick()
    {
        source.PlayOneShot(click);
    }
}
