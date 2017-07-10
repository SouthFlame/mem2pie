using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GeneralController : MonoBehaviour {

	public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.
	public Camera cam;

	public float currentX = 0f;
	public float currentY = 0f;

	private Vector3 origRot;
	private Vector3 localPos;
	//bool touchFromAngle = false;


	public float rotSpeed = 0.05f;
	public float direction = -1;

	public float speed = 10f;
	public static float hAngle = 90;
	float vAngle = 0;
	// Use this for initialization
	void Start ()
	{
		origRot = cam.transform.eulerAngles;
		localPos = cam.transform.localPosition;
		currentX = localPos.x; 
		currentY = localPos.z;
		//      currentX_loc = localPos.x;
		//      currentY_loc = localPos.z;

		hAngle = cam.transform.eulerAngles.y;
		vAngle = cam.transform.eulerAngles.x;

		//      nearPieButton.SetActive (false);
	}
	// Update is called once per frame
	void FixedUpdate () {

		//터치 스와이핑
		//1. 만약 손가락 하나가 입력된다면 그리고 터치된 손가락이 움직인다면
		//2. 변한 값만큼 카메라를 회전시킨다
		if (Input.touchCount==1 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			touchDeltaPosition = new Vector2(touchDeltaPosition.y, touchDeltaPosition.x * -1);
			if (Application.loadedLevel == 1)
			{
				//스와이핑 할 때, 기존의 상태에서 부터 시작
				//  - eulerAngle은 0~360도의 범위라서 clamp를 걸으면 에러가 난다.
				//  - 따라서 -90~90도의 범위로 바꾸어 hAngle에 저장해 준다.
				hAngle = cam.transform.eulerAngles.x;
				if(hAngle>90)
				{
					hAngle = hAngle - 360;
				}
				vAngle = cam.transform.eulerAngles.y;
				//스와이핑
				hAngle += touchDeltaPosition.x * speed * Time.deltaTime;
				vAngle += touchDeltaPosition.y * speed * Time.deltaTime;
				hAngle = Mathf.Clamp(hAngle, -90, 90);
				cam.transform.eulerAngles = new Vector3 (hAngle, vAngle, 0);
			}

			else if(Application.loadedLevel==2){ /*2D사진 씬에서 사용하는 터치컨트롤러 */
				currentX += touchDeltaPosition.y;
				currentY += touchDeltaPosition.x;

				cam.transform.localPosition = new Vector3 (currentX, 0, -currentY);
			}

			//if (cam.transform.eulerAngles.y > -90 && cam.transform.eulerAngles.y < 90)
			//{
			//    cam.gameObject.transform.Rotate(touchDeltaPosition * speed);
			//}
		}
		//if (Input.touchCount == 1) {

		//   foreach (Touch touch in Input.touches) {
		//      if (touch.phase == TouchPhase.Began) {
		//         initTouch = touch;
		//      } else if (touch.phase == TouchPhase.Moved) {
		//         //swiping
		//         float deltaX = initTouch.position.x - touch.position.x;      //touch에 대한, 따라서 x는 앵글에서는 Azimuth가 되~ 그래서 currentY를 해~
		//         float deltaY = initTouch.position.y - touch.position.y;
		//         currentX += deltaY * Time.smoothDeltaTime * rotSpeed * direction;
		//         currentY -= deltaX * Time.smoothDeltaTime * rotSpeed * direction;
		//         currentX = Mathf.Clamp (currentX, -89f, 89f);
		//         cam.transform.eulerAngles = new Vector3 (currentX, currentY, 0f);
		//      } else if (touch.phase == TouchPhase.Ended) {
		//         initTouch = new Touch ();
		//      }

		//   }
		//}

		if (Input.touchCount == 2) {
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// If the camera is orthographic...
			if (cam.orthographic)
			{
				// ... change the orthographic size based on the change in distance between the touches.
				cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

				// Make sure the orthographic size never drops below zero.
				cam.orthographicSize = Mathf.Max(cam.orthographicSize, 5.0f);
			}
			else
			{
				// Otherwise change the field of view based on the change in distance between the touches.
				cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

				// Clamp the field of view to make sure it's between 0 and 180.
				cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 5.0f, 140.0f);
			}
		}


		//Check whether closer to mem
		//      for (int i = 0; i < InputMemController.memList.Count; i++) {
		//
		//         aMemory = (Memory)InputMemController.memList [i];
		//         if ((Mathf.Abs(cam.transform.eulerAngles.y - aMemory.camY) < 20 
		//            || Mathf.Abs(cam.transform.eulerAngles.y - aMemory.camY) > 340)
		//            &&(Mathf.Abs(cam.transform.eulerAngles.x - aMemory.camX) < 20
		//               || Mathf.Abs(cam.transform.eulerAngles.x - aMemory.camX) > 340)) {
		//            memoryToOpen = aMemory;
		//            indexOfmemList = i;
		//            nearPieButton.SetActive (true);
		//            nearPieColor.GetComponent<Image>().color = new Color(aMemory.colorR, aMemory.colorG, aMemory.colorB, aMemory.colorA);
		//            break;
		//         }
		//         nearPieButton.SetActive (false);   
		//         nearPieColor.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		//      }


		//bydongwoo
		if(Application.platform == RuntimePlatform.Android)
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				SaveAndLoad.Save();
				Application.Quit();
			}
		}
	}


	public void Clickto0(){
		SaveAndLoad.Save();
		SceneManager.LoadScene (0);
	}





}