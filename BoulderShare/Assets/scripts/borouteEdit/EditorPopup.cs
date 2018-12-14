using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class EditorPopup : MonoBehaviour {
	[SerializeField]
	private Text textContent;
	private Action noProc = null;
	private Action yesProc = null;
	private Action closeProc = null;


	public void Open(Action yesAction, Action noAction, string content, Action closeAction = null){
		this.gameObject.SetActive(true);
		noProc = noAction;
		yesProc = yesAction;
		textContent.text = content;

		closeProc = closeAction ;
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

	public void PushCloseBtn(){
		if (closeProc != null){
			closeProc();
		}

		Close();
	}

	public void Close(){
		noProc = null;
		yesProc = null;
		closeProc = null;
		textContent.text = "";
		this.gameObject.SetActive(false);
	}
}
