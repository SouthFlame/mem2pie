using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable]
public class Memory{                  // 메모리 클래스야 우리의 저장할 것이라고 봐야지
	public float camX, camY;
	public string titleToMem, contextToMem;
	public float colorR=0, colorG=0, colorB=0, colorA=0;
	public float x,y,z;
	public Memory(string title, string context, float x, float y, Color pastelcolor){
		titleToMem = title;
		contextToMem = context;
		camX = x;
		camY = y;
		colorR = pastelcolor.r;
		colorG = pastelcolor.g;
		colorB = pastelcolor.b;
		colorA = pastelcolor.a;
	}
	public Memory(string title, string context,Vector3 vec, Color pastelcolor){
		titleToMem = title;
		contextToMem = context;
		x = vec.x;
		y = vec.y;
		z = vec.z;
		colorR = pastelcolor.r;
		colorG = pastelcolor.g;
		colorB = pastelcolor.b;
		colorA = pastelcolor.a;
	}
}

public class InputMemController : MonoBehaviour {
	public static ArrayList memList = new ArrayList();
	private float x, y;
	public InputField inputTitle, inputContext;
	public Memory mem;

	public GameObject memoryBoard;
	public GameObject deleteButton;
	public Color[] pastelColors;
	public Color aColor ;
	public Camera camMain;
	public ColorBlock coloBlo;



	public bool newButtonFlag = false;
	public bool editButtonFlag = false;
	public bool addEidtMemoryFlag = false;
	public bool addNewMemoryFlag = false;
	public bool deleteMemoryFlag = false;
	public bool addEidtMemoryFlag1 = false;
	public bool addNewMemoryFlag1 = false;
	public bool deleteMemoryFlag1 = false;

	//To Icon of NearPie
	public Memory memoryToOpen;
	public int indexOfmemList;


	// Use this for initialization
	void Start () {
		memoryBoard.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		if (memoryBoard.activeSelf == true)            //메모리 패널이 터치해서 조금 움직이면 꺼질 수 있게 하였다.
		if(Mathf.Abs(Input.GetTouch(0).deltaPosition.x) > 3 || Mathf.Abs(Input.GetTouch(0).deltaPosition.y) > 3)
			memoryBoard.SetActive(false);
	}


	// 메모리를 저장하려는 모든 행위에서 사용가능하다.
	public void ClicktoEnabled(){
		deleteButton.SetActive (false);
		newButtonFlag = true;
		inputTitle.placeholder.GetComponent<Text>().text = "10자 이내로 제목을 입력 해주세요...";
		inputTitle.text = "";
		inputContext.placeholder.GetComponent<Text>().text = "남기고 싶은 추억을 입력 해주세요...";
		inputContext.text = "";
		aColor = pastelColors[Random.Range(0,pastelColors.Length)];
		aColor.a = 0.8f;
		coloBlo = inputTitle.colors;
		coloBlo.normalColor=aColor;

		inputTitle.colors = coloBlo;
		inputContext.colors = coloBlo;

		//파이에선 좀더 진하게 하려구
		aColor.a = 0.7f;

		memoryBoard.SetActive(true);
	}
	public void ClicktoOpenMem(){
		editButtonFlag = true;
		deleteButton.SetActive (true);
		inputTitle.text = memoryToOpen.titleToMem;
		inputContext.text = memoryToOpen.contextToMem;
		aColor = new Color(memoryToOpen.colorR, memoryToOpen.colorG, memoryToOpen.colorB, memoryToOpen.colorA);
		aColor.a = 0.4f;
		coloBlo = inputTitle.colors;
		coloBlo.normalColor = aColor;

		inputTitle.colors = coloBlo;
		inputContext.colors = coloBlo;

		memoryBoard.SetActive(true);
	}


	// 메모리 패널에서 저장을 눌렀을 때
	public void ClicktoSave(){
		//Process to make pie
		if (newButtonFlag == true) {
			//mem = new Memory (inputTitle.text.ToString (), inputContext.text.ToString (), camMain.transform.eulerAngles.x, camMain.transform.eulerAngles.y, aColor);
			if (Application.loadedLevel == 1) {		
				mem = new Memory (inputTitle.text.ToString (), inputContext.text.ToString (), camMain.transform.eulerAngles.x, camMain.transform.eulerAngles.y, aColor);
			} else if (Application.loadedLevel == 2) {
				mem = new Memory (inputTitle.text.ToString (), inputContext.text.ToString (),MakeTag_2D_2.newvec, aColor);
			} /*2D사진 씬에서 사용하는 터치컨트롤러 */
			memList.Add(mem);
			addNewMemoryFlag = true;

			addNewMemoryFlag1 = true;
		}
		if (editButtonFlag == true) {
			memoryToOpen.titleToMem = inputTitle.text;
			memoryToOpen.contextToMem = inputContext.text;
			memList [indexOfmemList] = memoryToOpen;
			addEidtMemoryFlag = true;
			addEidtMemoryFlag1 = true;
		}

		//Flag초기화 및 메모리보드 안보이게 하기
		editButtonFlag = false;
		newButtonFlag = false;
		memoryBoard.SetActive(false);
	}


	//필수적으로 indexOfmemList를 지정해주어야 합니다!!!!
	public void ClicktoDelete(){

		deleteMemoryFlag = true;
		deleteMemoryFlag1 = true;
		editButtonFlag = false;
		memList.RemoveAt (indexOfmemList);
		if (memList.Count == 0) {
			memList.Clear ();
		}
		memoryBoard.SetActive(false);
	}


}