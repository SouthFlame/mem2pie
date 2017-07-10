using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//사용자가 클릭(터치)하는 위치에 tag를 생성해준다.
// 1. 한 지점을 TouchDelay 이상 터치하면,
// 2. 터치한 지점에 구, 메모창 생성
//  - tag만든 위치를 중앙에 보이게 카메라 회전
//  - 메모창 생성은 자동으로 버튼 클릭
// 3. 구 터치시, 메모 보이게
//  - 터치위치에서 ray를 쏜다
//  - ray에 맞은 것이 tag 일 때, 저장한 메모 보이기
//  - 메모 보이는 기능은 Button_NearPie를 누름
//  - 버튼을 누르지 않아도 가능하게
// 4. Tag 저장 가능하게
//  - memory list를 가져온다.
//  - 가져온 list를 그린다.
// 5. 아무것도 저장 된 것이 없다면 생성되었던 tag 삭제
//  - Editor 창이 켜졌는지 꺼졌는지 확인
//  - Editor 창이 켜졌다가 꺼졌을 때,
//  - memory list개수가 변하지 않았다면 tag삭제
// 6. tag로 이동 회전 속도 지정
//  - 회전시 속도 부여
// 7. 메모리와 tag를 연결시켜야 한다.
//  - 메모리형 tagMemoryList 생성
//  - 기존의 메모리 tag로 저장
//  - 그래프 그리는 것 tagMemoryList사용
//  - 위의 5번도 이를 이용해서 사용

public class TagContraller: MonoBehaviour
{
	public float ROTATE_SPEED = 20;//돌아가는 회전 속도 제어(셀프로 하시오)
	public float TouchDelay = 0.5f;
	public float TagRadius = 100f;
	public GameObject tagPrefab;
	public GameObject NOcllidertagPrefab;
	float currentTime = 0;
	bool delay_sw = false;
	GameObject newtag; 
	InputMemController controllTag;//button 없이
	//Button EditButton;
	//Button ShowButton;
	Memory aMemory;
	bool OnChangedEdit = false;//Editor 창이 켜져있는지 여부확인할때 사용
	int tagNum;//tagMemNum은 tag 모드에서 저장된 메모리 개수, tagNum은 그 안에 있는 메모리 개수
	List<GameObject> Tag;
	//  - 메모리형 tagMemoryList 생성
	//public static ArrayList tagMemoryList = new ArrayList();

	//public Text test;
	//public Text test1;
	//public Text test3;
	//public Text test4;
	//public Text test5;
	void Start () {
		//버튼 안보이게 하려면 CamCanvas Object에 붙어있는 스크립트 가져와야함.
		//그냥 스크립트 가져오면 안됨! 꼭 어딘가에 붙어있는 스크립트 가져와야함.
		//왜냐면 게임 오브젝트에 붙어있어야 그 스크립트 안에있는 Start랑 Update가 돌아감
		//controllTag = GameObject.Find("CamCanvas").GetComponent<InputMemController>();
		controllTag = GameObject.FindGameObjectWithTag("Canvas").GetComponent<InputMemController>();
		//EditButton = GameObject.Find("Button_Edit").GetComponent<Button>();//버튼있을떄
		//ShowButton = GameObject.Find("Button_NearPie").GetComponent<Button>();//버튼있을때
		Tag = new List<GameObject>();
		// 4. Tag 저장 가능하게
		//  - memory list를 가져온다.
		for(int i=0;i<InputMemController.memList.Count;i++)
		{
			
			aMemory = (Memory)InputMemController.memList[i];
			DrawTag(i,((Memory)InputMemController.memList[i]));
			//tagMemoryList.Add(aMemory);
			//DrawTag((Memory)tagMemoryList[i]);
		}
	}
	void Updatetag()
	{		

		for(int i=0;i<Tag.Count;i++)
		{
			Destroy (Tag [i]);
		}
		Tag.Clear();
		for(int i=0;i<InputMemController.memList.Count;i++)
		{

			aMemory = (Memory)InputMemController.memList[i];
			DrawTag(i,((Memory)InputMemController.memList[i]));
			//tagMemoryList.Add(aMemory);
			//DrawTag((Memory)tagMemoryList[i]);
		}
	}
	void Update()
	{
		//test.text = controllTag.indexOfmemList.ToString();
		//test3.text = controllTag.deleteMemoryFlag.ToString();
		////test5.text = controllTag.memoryToOpen.contextToMem+controllTag.memoryToOpen.titleToMem;
		////test4.text = controllTag.editButtonFlag.ToString();
		////test1.text = Tag.Count.ToString();
		////test.text = InputMemController.memList.Count.ToString();
		//test1.text = "";
		//for (int i = 0; i < InputMemController.memList.Count; ++i)
		//{
		//    Memory mem = (Memory)InputMemController.memList[i];
		//    test1.text += i + ":" + mem.titleToMem + mem.contextToMem + "\n";
		//}
		controllTag = GameObject.FindGameObjectWithTag("Canvas").GetComponent<InputMemController>();
		//test.text = "non hit";
		Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		// 1. 한 지점을 TouchDelay 이상 터치하면,
		//  - 한 지점에서 터치되고 있는 시간 저장

		// 3. 구 터치시, 메모 보이게
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 1000))
		{
			//  - ray에 맞은 것이 tag 일 때, 저장한 메모 보이기
			if (hitInfo.transform.gameObject.name.Contains("Tag"))
			{
				controllTag.newButtonFlag = false;
				//test.text = "hit";
				//카메라 이동
				//RotateCamera(hitInfo.transform.gameObject);
				//ShowButton.onClick.Invoke();//버튼있을때
				//controllTag.memoryToOpen = (Memory)InputMemController.memList[Tag.IndexOf(hitInfo.transform.gameObject)];

				//test.text = Tag.IndexOf(hitInfo.transform.gameObject).ToString();
				StartCoroutine(RotateCamera(hitInfo.transform.gameObject));
				controllTag.indexOfmemList = hitInfo.transform.gameObject.layer;
				controllTag.ClicktoOpenMem();//버튼 안 보일때도 가능하게
				//controllTag.memoryToOpen = null;
				//test3.text = controllTag.indexOfmemList.ToString();
			}
		}

		if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Stationary))
		{
			delay_sw = true;
		}
		else
		{
			currentTime = 0;
			delay_sw = false;
		}
		DelayTime(delay_sw);
		//  - 그 지점이 TouchDelay보다 크다면
		if (currentTime > TouchDelay)
		{
			controllTag.editButtonFlag = false;
			if (newtag)
				Destroy (newtag);
			newtag = Instantiate(NOcllidertagPrefab);
			//if (recentlyTag != null) { Destroy(recentlyTag); }
			// 2. 터치한 지점에 구, 메모창 생성
		

			newtag.transform.SetParent (this.transform,true);

			newtag.transform.position = ray.direction.normalized * TagRadius;

			//RotateCamera(tag);
			//EditButton.onClick.Invoke();//버튼있을때
			StartCoroutine(RotateCamera(newtag));
			newtag.transform.eulerAngles =  new Vector3(Camera.main.transform.eulerAngles.x,Camera.main.transform.eulerAngles.y,0);
			controllTag.ClicktoEnabled();//버튼 안 보일때도 가능하게
			currentTime = 0;
			OnChangedEdit = true;
			//test3.text = "SaveMode";
		}

	//	if(controllTag.deleteMemoryFlag1)
	//	{
	//		Destroy(Tag[controllTag.indexOfmemList]);
	//		Tag.Remove(Tag[controllTag.indexOfmemList]);
	//		controllTag.deleteMemoryFlag1 = false;
	//	}

		// 5. 아무것도 저장 된 것이 없다면 생성되었던 tag 삭제

		//if (tagMemNum < InputMemController.memList.Count)//저장이 되었다면, memList.Count가 더 큼
		//{
		//    tagMemNum = InputMemController.memList.Count;//tag 개수 업데이트
		//}

		////  - Editor 창이 켜졌는지 꺼졌는지 확인
		//if (controllTag.memoryBoard.activeSelf && (!hitInfo.transform.name.Contains("Tag")))
		//{
		//    OnChangedEdit = true;
		//}
		//-Editor 창이 켜졌다가 꺼졌을 때,
		if (OnChangedEdit)
		{	
			if (!controllTag.memoryBoard.activeSelf) {

			Destroy (newtag);
			OnChangedEdit = controllTag.memoryBoard.activeSelf; //그냥 지울때.
			} else {
			}
		}

		if (controllTag.addNewMemoryFlag1 == true) {
			Updatetag();
			controllTag.addNewMemoryFlag1 = false;
		}
		if (controllTag.addEidtMemoryFlag1 == true) {
			Updatetag();
			controllTag.addEidtMemoryFlag1 = false;
		}
		if (controllTag.deleteMemoryFlag1 == true) {
			Updatetag();
			controllTag.deleteMemoryFlag1 = false;
		}
	}
	void DelayTime(bool delay_sw)
	{
		if (delay_sw)
		{
			currentTime += Time.deltaTime;
			//test.text = currentTime.ToString();
		}
	}
	// 4. tag로 이동 회전 속도 지정
	//  - 회전시 속도 부여
	IEnumerator RotateCamera(GameObject tag)
	{
		while (true)
		{
			Vector3 eulerTarget = Quaternion.LookRotation(tag.transform.position).eulerAngles;//eulerTarget은 을 향한 각도값
			Vector3 MainCamera = Camera.main.transform.rotation.eulerAngles;
			Quaternion target=new Quaternion(0,0,0,0);
			if (eulerTarget.x > 90 && eulerTarget.x < 270)
			{
				eulerTarget.x = 180 - eulerTarget.x;
				eulerTarget.y += 180;
			}
			target.eulerAngles = eulerTarget;
			Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, target, ROTATE_SPEED * Time.deltaTime);
			if((Camera.main.transform.eulerAngles-target.eulerAngles).magnitude<0.1f||currentTime>1.4f)
			{
				break;
			}
			//Vector3 dir = (target - Camera.main.transform.eulerAngles).normalized;//회전할 방향 저장
			//if (dir.magnitude < 0.1f) { break; }
			//Camera.main.transform.eulerAngles += dir * ROTATE_SPEED * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		//  - tag만든 위치를 중앙에 보이게 카메라 회전
		//Camera.main.transform.eulerAngles = target.eulerAngles;
		yield return null;
	}
	//  - 가져온 list를 그린다.
	public void DrawTag(int i,Memory mem)
	{
		GameObject tag = Instantiate(tagPrefab);
		tag.transform.SetParent (transform,false);
		tag.transform.eulerAngles = new Vector3(mem.camX, mem.camY, 0);
		tag.transform.position = tag.transform.forward.normalized * TagRadius;
		tag.layer = i;
		Tag.Add(tag);
	}
}