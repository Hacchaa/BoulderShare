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
	
	public void AddComment(){
		string txt = input.text;


		if (!(txt == null || txt.Trim() == "")){
			GameObject obj = Instantiate(commentPrefab, root);
			obj.transform.Find("Text").GetComponent<Text>().text = txt;
		}
		input.text = "";

		addBtn.SetSiblingIndex(addBtn.GetSiblingIndex()+1);
	}
}
