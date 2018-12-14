using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlaceDD2 : MonoBehaviour {
	[SerializeField]
	private InputField inputField;
	[SerializeField]
	private Dropdown dd;

	public void CreateOptions(List<string> list){
		dd.ClearOptions();
		//リストの最後に空白を挿入
		list.Add("");
		dd.AddOptions(list);
		//空白が選択されるようにする
		dd.value = list.Count;
	}

	public void OnValueChanged(int value){
		if(String.IsNullOrEmpty(dd.options[value].text)){
			return ;
		}
		inputField.text = dd.options[value].text;
	}
	
	public string GetPlace(){
		return inputField.text;
	}

	public void SetPlace(string str){
		inputField.text = str;
	}
}
