using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropdown : MonoBehaviour {


	public RectTransform container;
	public bool isOpen;
	// Use this for initialization
	void Start () {
		container = transform.FindChild ("Container").GetComponent<RectTransform> ();
		isOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
			Vector3 scale = container.localScale;
			scale.x = Mathf.Lerp (scale.x,isOpen ? 1:0,Time.deltaTime*30);
			container.localScale = scale;
	
	}

	public void OnclickSelectMode(){
		isOpen = !isOpen;
	}
}
