using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSceneComment : MonoBehaviour, IUIComponent {
	[SerializeField]
	private List<GameObject> externalUIComponents;


	public void ShowProc(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		this.gameObject.SetActive(true);
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		this.gameObject.SetActive(false);
	}

}
