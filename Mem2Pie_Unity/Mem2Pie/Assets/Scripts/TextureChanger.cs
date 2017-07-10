using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextureChanger : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		if(Application.loadedLevel==1)
		{
		this.GetComponent<MeshRenderer>().material.mainTexture = Gallerytest.galleryImage;
		this.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (-1, 1);
		}
		else
		{
			this.GetComponent<RawImage>().texture= Gallerytest.galleryImage;
			this.GetComponent<RawImage> ().SetNativeSize ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
