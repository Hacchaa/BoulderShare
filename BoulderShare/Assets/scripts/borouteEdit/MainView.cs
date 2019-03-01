using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView: SEComponentBase{
	[SerializeField]private List<GameObject> externalUIComponents;
	[SerializeField]private ScreenTransitionManager trans;
	[SerializeField] private MakeAttemptTree makeAT;

	public override void ShowProc(){
		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
	}

	public override void HideProc(){
		Hide();
	}

	public override void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
	}

	public void ToMakeAT(){
		makeAT.Init();
		trans.Transition("EditWallMark");
	}

	public void ToATV(){
		trans.Transition("AttemptTreeView");
	}

	public void ToPost(){
		trans.Transition("Post");
	}

	public void ToTry(){
		trans.Transition("TryView");
	}

	public void To3DSetting(){
		trans.Transition("ThreeDFirstSettingView");
	}
}
