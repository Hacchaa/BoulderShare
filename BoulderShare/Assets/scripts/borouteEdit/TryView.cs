using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TryView : MonoBehaviour, IUIComponent {
	[SerializeField]
	private GameObject[] frames;
	private int curFrameIndex;
	[SerializeField]
	private GameObject congFrame;
	[SerializeField]
	private ScreenTransitionManager stManager;
	[SerializeField]
	private HScenes2 hScenes;
	[SerializeField]
	private FailedListView fListView;
	[SerializeField]
	private GameObject failedList_ParentObj;


	public void ShowProc(){
		curFrameIndex = 0;

		this.gameObject.SetActive(true);
		frames[curFrameIndex].SetActive(true);
	}

	public void HideProc(){
		curFrameIndex = 0;
		Hide();
	}

	public void Hide(){
		foreach(GameObject obj in frames){
			obj.SetActive(false);
		}
		this.gameObject.SetActive(false);
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
		stManager.Transition("Post");
	}
	public void ToCong(){
		frames[curFrameIndex].SetActive(false);
		congFrame.SetActive(true);
	}

	public void ToATV(){
		stManager.Transition("AttemptTreeView");
	}

	public void ReObsProc(){
		hScenes.RegistCurHScenes();

		List<string> fList = new List<string>();
		//parentObjの子供が持っているテキストをlistにしてhscenesに渡す
		foreach(Transform child in failedList_ParentObj.transform){
			Toggle tog = child.GetComponent<Toggle>();
			if (tog.isOn){
				fList.Add(child.transform.Find("Label")
					.gameObject.GetComponent<Text>().text);
				tog.isOn = false;
			}
		}
		hScenes.SetFailedList(fList);
		//flistに更新が必要なことを伝える
		fListView.SetIsUpdateNeed(true);
		ToATV();
	}
}
