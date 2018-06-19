using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldTypeControl : MonoBehaviour {
	private static int hType;
	public enum HoldType {None, Normal, Foot};

	void Start(){
		hType = 0;
	}
	public void ChangeHoldType(int type){
		hType = type;
	}

	public int GetHoldType(){
		return hType;
	}

	public void Call(){
		Debug.Log("type:"+hType);
	}
}
