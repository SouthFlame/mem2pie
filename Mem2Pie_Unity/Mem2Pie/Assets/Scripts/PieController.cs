using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PieController : MonoBehaviour {
	public Image wedgePrefab;
	public List<GameObject> arrNewWedge = new List<GameObject>();
	private Memory aMemory;
	public GameObject cam;
	float x,y;
	float moveX, moveY;
	bool moveCamFlag = false;


	public GameObject nearPieButton_Land;      //이것은 실제 뷰에서 나온 것이다.
	public GameObject nearPieButton_Port;
	private GameObject nearPieButton;
	public GameObject nearPieColor_Land;      //이것은 실제 뷰에서 나온 것이다.
	public GameObject nearPieColor_Port;
	private GameObject nearPieColor;

	//1. 카메라 각도
	//public GameObject cam2;
	Vector3 MainCamera;
	//2. target 각도
	//   - euler로 조절
	public Vector3 eulerTarget;
	Quaternion target;
	//   - 속도 설정
	float thetaY, thetaX;
	public float dir;//두 점 사이 거리
	public float FIRST_SPEED=30f;
	public float HIGH_SPEED=100f;
	public float LOW_SPEED=30f;
	public float FAST_DE=10f;
	public float LOW_DE=5f;
	public float LOW_DISTANCE = 30f;
	float [] DISTANCE=new float[]{360f,60f,30f,10f};
	float[] SPEED = new float[]{300f,200f,100f,30f ,10f};
	public float speed;


	//2. Pie를 보게 하는 것
	bool gotoPie = false;
	private float currentX, currentY;

	//3. To Icon of NearPie
	public Memory mem;

	InputMemController inputMemController;

	void Start () {
		nearPieButton_Port.SetActive (false);
		nearPieButton_Land.SetActive (false);
		updatePie ();
	}
	void Update () {
		//파이를 눌러서 화면을 이동시켜 준다.
		if (moveCamFlag == true) {
			eulerTarget = new Vector3 (moveX, moveY, 0f);
			MainCamera = cam.transform.rotation.eulerAngles;

			if (eulerTarget.x > 90 && eulerTarget.x < 270) {
				eulerTarget.x = 180 - eulerTarget.x;
				eulerTarget.y += 180;
			}

			target.eulerAngles = eulerTarget;
			//두 점 사이 거리
			dir=(MainCamera-eulerTarget).magnitude;
			thetaY = Mathf.Abs (MainCamera.y - eulerTarget.y);
			if (thetaY > 180)
				thetaY = 360 - thetaY;

			thetaX = Mathf.Abs (MainCamera.x - eulerTarget.x);
			if (thetaX > 180)
				thetaX = 360 - thetaX;

			//rotation -y direction is error cause dir is increasing
			//         if(dir>yy_){
			//            target.y += 360;
			//         }
			//         yy_ = dir;
			if (dir <= 0.5) {
				speed = SPEED[0];
				moveCamFlag = false;
			}
			//   - 두 점 이동시 빠르게->느리게 (dir:0~50-->low, dir:50~-->high)
			if (thetaY+thetaX>DISTANCE[0]) {
				speed = SPEED [0];
			} else if (thetaY+thetaX>DISTANCE[1]) {
				speed = SPEED [1];
			} else if (thetaY+thetaX > DISTANCE [2]) {
				speed = SPEED [2];
			} else if (thetaY+thetaX > DISTANCE [3]) {
				speed = SPEED [3];
			} else {
				speed = SPEED [4];
			}
			print ("speed--->"+speed);

			//3. 회전
			cam.transform.rotation=Quaternion.RotateTowards(cam.transform.rotation,target,speed*Time.deltaTime);
		}
///////////////////////////////////여기까지 파이버튼 눌러 해당 앵글로 이동



		//화면 방향에 따라 Inputcontroller 잡아주는 코드
		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
			inputMemController = GameObject.Find ("CamCanvas_Land").GetComponent<InputMemController>();
			nearPieButton = nearPieButton_Land;
			nearPieColor = nearPieColor_Land;
		}
		else if (Screen.orientation == ScreenOrientation.Portrait) {
			inputMemController = GameObject.Find ("CamCanvas_Port").GetComponent<InputMemController>();
			nearPieButton = nearPieButton_Port;
			nearPieColor = nearPieColor_Port;
		}



		//파이를 보게 해주는 코드
		if (gotoPie == true) {
			currentX = cam.transform.eulerAngles.x;
			currentY = cam.transform.eulerAngles.y;
			currentX++;
			if (currentX > 359) {
				currentX = 0;
			}
			if (89 < currentX && currentX <91) {
				GeneralController.hAngle = 90f;
				gotoPie = false;
			}
			cam.transform.eulerAngles = new Vector3 (currentX, currentY, 0f);
		}


		//파이 가까이 갔을 때.
		for (int i = 0; i < InputMemController.memList.Count; i++) {
			mem = (Memory)InputMemController.memList [i];
			if ((Mathf.Abs(cam.transform.eulerAngles.y - mem.camY) < 10 
				|| Mathf.Abs(cam.transform.eulerAngles.y - mem.camY) > 350)
				&&(Mathf.Abs(cam.transform.eulerAngles.x - mem.camX) < 10
					|| Mathf.Abs(cam.transform.eulerAngles.x - mem.camX) > 350)) {
				inputMemController.memoryToOpen = mem;
				inputMemController.indexOfmemList = i;
				nearPieButton.SetActive (true);                                                                           /////////////
				nearPieColor.GetComponent<Image>().color = new Color(mem.colorR, mem.colorG, mem.colorB, mem.colorA);                     ///////////////////////////

				break;
			}
			nearPieButton.SetActive (false);   
			nearPieColor.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		}


		//updatePie();
		//파이 그려주는 부분
			if (inputMemController.addNewMemoryFlag == true) {
				updatePie ();
				inputMemController.addNewMemoryFlag = false;
			}
			if (inputMemController.addEidtMemoryFlag == true) {
				updatePie ();
				inputMemController.addEidtMemoryFlag = false;
			}
			if (inputMemController.deleteMemoryFlag == true) {
				updatePie ();
				nearPieButton.SetActive (false);
				inputMemController.deleteMemoryFlag = false;
			}
	}



	// 파이를 최신화 시켜준다.
	public void updatePie() {
		for (int i = 0; i < arrNewWedge.Count; i++) { 
			Destroy(arrNewWedge[i]);
			//arrNewWedge.RemoveAt(i);  
			//다 지워 주고
		}
		arrNewWedge.Clear();
		for (int i = 0; i < InputMemController.memList.Count; i++) {      // 다시 다 그준다. 그서 memList를 열람하여
			aMemory = (Memory)InputMemController.memList [i];            // aMemory에 넣어줘서         
			NewGraph (aMemory, 0.125f);                              // 뉴그래프 함수를 이용하여 다 렌더링 해준다.
		}
	}

	//파이 그려주는 역할이다.
	public void NewGraph(Memory mem, float fillAmount)
	{  
		//Make Pie Image
		GameObject newButtonObj = new GameObject ("Button" + mem.camY);
		newButtonObj.transform.SetParent(transform, false);
		//newButtonObj.transform.position = new Vector3(0f,-290f,0f);
		newButtonObj.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, -(mem.camY)));
		newButtonObj.layer = 5;      // UI Layer
		Button newButton = newButtonObj.AddComponent<Button> ();
		CanvasRenderer canvasButton = newButtonObj.AddComponent<CanvasRenderer> ();
		RectTransform recTransButton = newButtonObj.AddComponent<RectTransform> ();
		Image imageButton = newButtonObj.AddComponent<Image> ();
		recTransButton.pivot = new Vector2(0.5f,-0.25f);
		recTransButton.sizeDelta = new Vector2 (40f,120f);
		newButton.targetGraphic = imageButton;
		imageButton.color = new Color (0,0,0,0);
		arrNewWedge.Add(newButtonObj);

		Image newWedge=Instantiate(wedgePrefab) as Image;
		newWedge.transform.SetParent(newButtonObj.transform, false);
		newWedge.color = new Color(mem.colorR,mem.colorG,mem.colorB,mem.colorA);
		newWedge.fillAmount = fillAmount;

		newWedge.transform.position = new Vector3(0f,-400f,0f);
		newWedge.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, -(mem.camY-fillAmount*180)));

		GameObject newTextObj = new GameObject ("Text");
		Text newText = newTextObj.AddComponent<Text>();
		newText.transform.SetParent (newButtonObj.transform, false);

		newText.rectTransform.position = new Vector3(newText.rectTransform.position.x,-395f,newText.rectTransform.position.z);
		newText.text = mem.titleToMem;
		Font SDMiSaengFont = (Font)Resources.Load("SDMiSaeng", typeof(Font));
		newText.font = SDMiSaengFont;
		newText.fontSize = 21;
		newText.alignment = TextAnchor.MiddleCenter;
		newText.material = SDMiSaengFont.material;
		newText.color =new Color(0.125f,0.125f,0.125f,1);
		newText.rectTransform.Rotate(new Vector3(0f,0f,90f));
		newText.transform.SetParent(newButton.transform, true);
		newText.rectTransform.sizeDelta = new Vector2 (100f,30f);
		newText.lineSpacing = 1f;
		newButton.onClick.AddListener(delegate(){this.MoveCam(mem.camX,mem.camY);});
	}


	public void MoveCam (float x, float y) {
		moveX = x;
		moveY = y;
		moveCamFlag = true;
	}

	public void ClickGoPie(){
		gotoPie = true;
		//cam.transform.eulerAngles.x = new Vector3 (0f, 90f, 0f);
	}

	//nearPieButton이 액티브 되었을때


}