using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownOpe : MonoBehaviour {
	private Dropdown dd;

	// Use this for initialization
	void Start () {
		dd = GetComponent<Dropdown>();
	}
	
	public void ValueChanged(int v){
		dd.value = v;
	}

	public string GetCurrentOption(){
		return dd.options[dd.value].text;
	}

	public int GetCurrentValue(){
		return dd.value;
	}

	public void SetCurrentValue(int v){
		dd.value = v;
	}
}
