using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class Gallerytest :MonoBehaviour {
	public static Texture2D galleryImage;
	// Use this for initialization
	bool isGalleryImageLoaded= false;

	bool b_image2D= false;
	public static WWW www;
	public static  string myfilePath="nullnull";
    bool buttonClicked = true;
	void Start () {

	}
	void OnGUI()
	{
		if (isGalleryImageLoaded) {
			//GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), galleryImage);
			if (b_image2D) {
				SceneManager.LoadScene (2);
			}
				else {
				SceneManager.LoadScene (1);
			}
		}
		if (buttonClicked) {
			buttonClicked=false;
			isGalleryImageLoaded = false;
			AndroidJavaClass ajc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaClass ajo= new AndroidJavaClass("com.example.kimdongwoo_lab.unityplug.UnityBinder");
			//갤러리를 연다
			ajo.CallStatic("OpenGallery",ajc.GetStatic<AndroidJavaObject>("currentActivity"));

		}

		if(Application.platform == RuntimePlatform.Android)
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				// 할꺼 하셈
				Application.Quit();
			}
		}
		if (www != null && www.isDone) {
			galleryImage = new Texture2D (www.texture.width, www.texture.height);
			if (www.texture.width / www.texture.height == 2) {
				b_image2D = false;
			} else {
				b_image2D = true;
			}
		
			galleryImage.SetPixels32 (www.texture.GetPixels32 ());
			galleryImage.Apply ();
			www = null;
			isGalleryImageLoaded = true;

		}

	}

	public void OnPhotoPick(string filePath)
	{
		myfilePath = filePath;
		Debug.Log(filePath);
		www = new WWW ("file://" + filePath);
		SaveAndLoad.Load();
	}
	public void Click()
	{
		SaveAndLoad.Save();
		buttonClicked=true;
	}

}
