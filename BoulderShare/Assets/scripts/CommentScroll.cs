using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CommentScroll : MonoBehaviour {
	public Transform root;
	public GameObject commentPrefab;
	public InputField input;
	public Transform addBtn;
	private List<string> comments ;

	void Start(){
		comments = new List<string>();
	}

	public void Reset(){
		comments = new List<string>();
		foreach (Transform child in root ){
			GameObject.Destroy(child.gameObject);
		}
	}
	
	public void AddComment(){
		string txt = input.text;
		comments.Add(txt);

		if (!(txt == null || txt.Trim() == "")){
			GameObject obj = Instantiate(commentPrefab, root);
			obj.transform.Find("Text").GetComponent<Text>().text = txt;
		}
		input.text = "";

		addBtn.SetSiblingIndex(addBtn.GetSiblingIndex()+1);
	}

	//参照渡し
	public List<string> GetComments(){
		return comments;
	}

	public void SetComments(List<string> com){
		Reset();
		foreach(string str in com){
			input.text = str;
			AddComment();
		}
	}
}
