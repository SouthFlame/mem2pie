using UnityEngine;
using System.Collections;

public class Ori_Controller : MonoBehaviour {
	public GameObject canvusLandscape;
	public GameObject canvusPortrait;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update() {
		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
			canvusPortrait.SetActive (false);
			canvusLandscape.SetActive (true);
		}
		else if (Screen.orientation == ScreenOrientation.Portrait) {
			canvusPortrait.SetActive (true);
			canvusLandscape.SetActive (false);
		}
	}
}
