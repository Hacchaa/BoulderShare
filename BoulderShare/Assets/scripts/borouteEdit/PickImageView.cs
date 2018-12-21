using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kakera;

public class PickImageView : MonoBehaviour, IUIComponent {
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

	public void ShowProc(){
		foreach(GameObject obj in internalUIComponents){
			obj.SetActive(true);
		}
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		gameObject.SetActive(true);
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		foreach(GameObject obj in internalUIComponents){
			obj.SetActive(false);
		}
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		gameObject.SetActive(false);
	}

	public void ToATV(){
		twoDWallImage.RotateWallTexture();
		trans.Transition("EditWallMark");
	}

}
