using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoRouteInfo : MonoBehaviour {
	private List<string> pList;

	void Awake(){
		DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
	}

	public void SetPlaceList(List<string> p){
		pList = p;
	}

	public List<string> GetPlaceList(){
		return pList;
	}
}
