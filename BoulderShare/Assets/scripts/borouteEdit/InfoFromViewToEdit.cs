using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoFromViewToEdit : MonoBehaviour {
	private bool isNew ;
	private List<string> pList;
	private string dirName;

	void Awake(){
		DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
		isNew = true;
	}

	public void SetPlaceList(List<string> p){
		pList = p;
	}

	public List<string> GetPlaceList(){
		return pList;
	}

	public void SetIsNew(bool b){
		isNew = b;
	}

	public bool IsNew(){
		return isNew;
	}

	public string GetDirName(){
		return dirName;
	}

	public void SetDirName(string str){
		dirName = str;
	}
}
