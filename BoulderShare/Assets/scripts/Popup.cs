using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class Popup : MonoBehaviour {
	[SerializeField]
	private Text main;
	private Action noProc = null;
	private Action yesProc = null;



	public void Open(Action yesAction, Action noAction, string content){
		this.gameObject.SetActive(true);
		noProc = noAction;
		yesProc = yesAction;
		main.text = content;
	}
	
	public void PushNoBtn(){
		if (noProc != null){
			noProc();
		}
		Close();
	}

	public void PushYesBtn(){
		if (yesProc != null){
			yesProc();
		}
		Close();
	}

	public void Close(){
		noProc = null;
		yesProc = null;
		this.gameObject.SetActive(false);
	}
}
