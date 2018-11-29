using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Try : MonoBehaviour {
	[SerializeField]
	private GameObject[] frames;
	private int curFrameIndex;
	[SerializeField]
	private Post post;
	[SerializeField]
	private ThreeDModel model;
	[SerializeField]
	private GameObject congFrame;
	[SerializeField]
	private GameObject failedList_ParentObj;
	[SerializeField]
	private Issues issues;

	void Awake(){
		curFrameIndex = 0;

		foreach(GameObject obj in frames){
			obj.SetActive(false);
		}
		congFrame.SetActive(false);
	}

	public void UpdateFailedList(){
		List<string> list = new List<string>();

		//parentObjの子供が持っているテキストをlistにしてissuesに渡す
		foreach(Transform child in failedList_ParentObj.transform){
			Toggle tog = child.GetComponent<Toggle>();
			if (tog.isOn){
				list.Add(child.transform.Find("Label")
					.gameObject.GetComponent<Text>().text);
				tog.isOn = false;
			}
		}
		issues.Regist(list);
	}

	public void ClearFailedList(){
		issues.Regist(new List<string>());
	}

	public void Open(){
		model.ChangeMode((int)ThreeDModel.Mode.DEFAULT);
		this.gameObject.SetActive(true);
		frames[curFrameIndex].SetActive(true);
	}

	public void Close(){
		frames[curFrameIndex].SetActive(false);
		this.gameObject.SetActive(false);
		curFrameIndex = 0;
		model.ChangeMode((int)ThreeDModel.Mode.WINDOW);
	}

	public void Next(){
		if (curFrameIndex >= frames.Length - 1){
			return ;
		}
		curFrameIndex++;
		frames[curFrameIndex].SetActive(true);
		frames[curFrameIndex-1].SetActive(false);
	}

	public void Prev(){
		if (curFrameIndex <= 0){
			return ;
		}
		curFrameIndex--;
		frames[curFrameIndex].SetActive(true);
		frames[curFrameIndex+1].SetActive(false);
	}

	public void ToPost(){
		frames[curFrameIndex].SetActive(false);
		congFrame.SetActive(false);
		Close();
		post.Open();
	}

	public void ToCong(){
		frames[curFrameIndex].SetActive(false);
		congFrame.SetActive(true);
	}
}
