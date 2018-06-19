using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comment : MonoBehaviour {

	private InputField input;
	private string comment;

	// Use this for initialization
	void Start () {
		input = GetComponent<InputField>();
		comment = null;
	}
	
	public string GetComment(){
		return comment;
	}

	public void SetComment(string com){
		if (input != null){
			input.text = com;
		}
	}

	public void Complete(){
		comment = input.text;
		SetComment("");
	}

}
