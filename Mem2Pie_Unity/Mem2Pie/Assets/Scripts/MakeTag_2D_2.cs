//2D tag모드
//1. 0.5초동안 터치시 맞은 위치에 Tag 생성
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class MakeTag_2D_2 : MonoBehaviour
{


	public float TagRadius = 100f;
	public static Vector3 newvec;

	float currentTime = 0;
	public GameObject tagPrefab;

	InputMemController controllTag;//button 없이
	Memory aMemory;
	List<GameObject> Tag = new List<GameObject>();
	bool OnChangedEdit = false;//Editor 창이 켜져있는지 여부확인할때 사용

	public Text test;

	void Start()
	{  

		//Hierarchy에서 태그해줘야되에에!!!!!~!!~!~~~~!!!
		controllTag = GameObject.FindGameObjectWithTag("Canvas").GetComponent<InputMemController>();

		for(int i=0;i<InputMemController.memList.Count;++i)
		{
			DrawTag(((Memory)InputMemController.memList[i]));
		}

	}
	void Update()
	{
		test.text = currentTime.ToString();


		//Hierarchy에서 태그해줘야되에에!!!!!~!!~!~~~~!!!
		controllTag = GameObject.FindGameObjectWithTag("Canvas").GetComponent<InputMemController>();


		Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		RaycastHit hitInfo;

//태그를 클릭 했을 때.
		if (Physics.Raycast (ray, out hitInfo, 1000)) {
			//  - ray에 맞은 것이 tag 일 때, 저장한 메모 보이기
			if (hitInfo.transform.gameObject.name.Contains ("Tag") && (Tag.Count == InputMemController.memList.Count)) {
				controllTag.newButtonFlag = false;
				//test.text = "hit";
				//카메라 이동
				//RotateCamera(hitInfo.transform.gameObject);
				//ShowButton.onClick.Invoke();//버튼있을때
				controllTag.memoryToOpen = (Memory)InputMemController.memList [Tag.IndexOf (hitInfo.transform.gameObject)];
				controllTag.indexOfmemList = Tag.IndexOf(hitInfo.transform.gameObject);
				controllTag.ClicktoOpenMem();
			}
		}



		//터치 카운트(새로 만들준비)
		if (Input.touchCount==1&&Input.GetTouch(0).phase==TouchPhase.Stationary)
		{
			currentTime += Time.deltaTime;
		}
		else
		{
			currentTime = 0;
		}


		//태그 새로 만들 때.
		if(currentTime>0.5f)
		{
			controllTag.editButtonFlag = false;
			if (Tag.Count != InputMemController.memList.Count)
			{
				Destroy(Tag[Tag.Count - 1]);
				Tag.Remove(Tag[Tag.Count - 1]);
			}
			if (Physics.Raycast(ray, out hitInfo, 1000) && hitInfo.transform.gameObject.name.Contains("Floor"))
			{
				GameObject tag = Instantiate(tagPrefab);
				Tag.Add(tag);
				Tag[Tag.Count-1].transform.position = hitInfo.point;
				newvec = Tag [Tag.Count - 1].transform.position;
				controllTag.ClicktoEnabled();
				currentTime = 0;
				OnChangedEdit = true;
			}
		}


		//태그 제거할 때
		if(controllTag.deleteMemoryFlag)
		{
			Destroy(Tag[controllTag.indexOfmemList]);
			Tag.RemoveAt(controllTag.indexOfmemList);
			controllTag.deleteMemoryFlag = false;
		}

		if (OnChangedEdit)
		{
			if (!controllTag.memoryBoard.activeSelf)
			{
				if(Tag.Count!= InputMemController.memList.Count)
				{
					Destroy(Tag[Tag.Count - 1]);
					Tag.Remove(Tag[Tag.Count - 1]);
				}
				OnChangedEdit = controllTag.memoryBoard.activeSelf;
			}
			else
			{
				if(Tag.Count==InputMemController.memList.Count)
				{
					controllTag.memoryBoard.SetActive(false);
				}
			}
		}

	}



	public void DrawTag(Memory mem)
	{
		GameObject tag = Instantiate(tagPrefab);
		Tag.Add(tag);
		tag.transform.position = new Vector3 (mem.x, mem.y, mem.z);
		//		tag.transform.position = tag.transform.forward.normalized * TagRadius;
	}
}
