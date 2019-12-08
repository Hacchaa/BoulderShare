using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnBtn : MonoBehaviour {
	[SerializeField]
	private BoRouteLSManager bManager;
	[SerializeField]
	private Popup popup;

	public void Return(){
		string str = "ボルートを一時保存しますか？";
		popup.Open(yes, no, str);
	}

	private void yes(){
		bManager.SaveTemporary();
		SceneManager.LoadScene("routeview");
	}

	private void no(){
		SceneManager.LoadScene("routeview");
	}
}
