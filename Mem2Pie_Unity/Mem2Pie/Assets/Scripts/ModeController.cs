using UnityEngine;
using System.Collections;

public class ModeController : MonoBehaviour 
{
	public int modeSelet = 1;

	public GameObject PieButtonGroup_Land;      //다른모드에서는 꺼주고 싶어서
	public GameObject PieButtonGroup_Port;
	public GameObject PieMode;      //다른모드에서는 꺼주고 싶어서
	public GameObject TagMode_f;
	public GameObject TagA;

//	// Use this for initialization
	void Start () {

		//PieButtonGroup_Land.SetActive(false);      //다른모드에서는 꺼주고 싶어서
		//PieButtonGroup_Port.SetActive(false);
		//PieMode.SetActive(false);      //다른모드에서는 꺼주고 싶어서
		//TagMode.SetActive(true);
		//TagA=Instantiate(TagMode_f);

		PieButtonGroup_Land.SetActive(true);      //다른모드에서는 꺼주고 싶어서
		PieButtonGroup_Port.SetActive(true);
		PieMode.SetActive(true);      //다른모드에서는 꺼주고 싶어서
		Destroy(TagA);
	}

	// Update is called once per frame
	void Update () {
	}
		
	public void OnclickedPieMode(){
		PieButtonGroup_Land.SetActive(true);      //다른모드에서는 꺼주고 싶어서
		PieButtonGroup_Port.SetActive(true);
		PieMode.SetActive(true);
		//PieMode.GetComponentInChildren<PieController> ().updatePie();//다른모드에서는 꺼주고 싶어서
		Destroy(TagA);
		//.SetActive(false);
		//GameObject[] gos = GameObject.FindGameObjectsWithTag("Tag");
		//loop through the returned array of game objects and set each to active false
		//foreach (GameObject go in gos) {
		//	go.SetActive(false);
		//}
	}

	public void OnclickedTagMode(){
		PieButtonGroup_Land.SetActive(false);      //다른모드에서는 꺼주고 싶어서
		PieButtonGroup_Port.SetActive(false);
		PieMode.SetActive(false);      //다른모드에서는 꺼주고 싶어서
		//TagMode.SetActive(true);
		Destroy(TagA);
		TagA=Instantiate(TagMode_f);

	//	TagMode.GetComponent<YJY_MakeTag>().DrawAllTag();
	}

}
