using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TryView : SEComponentBase{
	[SerializeField] private GameObject[] frames; 
	[SerializeField] private GameObject congFrame;
	[SerializeField] private ScreenTransitionManager stManager;
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private GameObject failedList_ParentObj;
	private int curFrameIndex;

	public override void OnPreShow(){
		curFrameIndex = 0;
	}

	public override void OnPreHide(){
		curFrameIndex = 0;
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
		stManager.Transition(ScreenTransitionManager.Screen.Post);
	}
	public void ToCong(){
		frames[curFrameIndex].SetActive(false);
		congFrame.SetActive(true);
	}

	public void ToMainView(){
		stManager.Transition(ScreenTransitionManager.Screen.MainView);
	}

	public void ToATFV(){
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.Failure;
		stManager.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
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
		//hScenes.SetFailedList(fList);
		ToMainView();
	}
}
