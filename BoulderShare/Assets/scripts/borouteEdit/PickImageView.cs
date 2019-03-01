using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kakera;

public class PickImageView : SEComponentBase{
	[SerializeField]
	private List<GameObject> internalUIComponents;
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private PickerController2 pc;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private TwoDWallImage twoDWallImage;

	public override void ShowProc(){
		foreach(GameObject obj in internalUIComponents){
			obj.SetActive(true);
		}
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		gameObject.SetActive(true);
	}

	public override void HideProc(){
		Hide();
	}

	public override void Hide(){
		foreach(GameObject obj in internalUIComponents){
			obj.SetActive(false);
		}
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		gameObject.SetActive(false);
	}

	public void ToMainView(){
		twoDWallImage.RotateWallTexture();
		trans.Transition("MainView");
	}

}
