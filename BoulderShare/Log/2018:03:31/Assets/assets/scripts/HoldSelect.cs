using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldSelect : MonoBehaviour {
	private GameObject observer;
	private HoldTypeControl holdTypeControl;

	void Start(){
		observer = GameObject.Find("Observer");
		holdTypeControl = observer.GetComponent<HoldTypeControl>();
	}	
	public void ChangeHoldType(bool type){
		string name = gameObject.name;
		if (!type){
			holdTypeControl.ChangeHoldType((int)HoldTypeControl.HoldType.None);
		}else if (name == "Normal"){
			holdTypeControl.ChangeHoldType((int)HoldTypeControl.HoldType.Normal);
		}else if(name == "Foot"){
			holdTypeControl.ChangeHoldType((int)HoldTypeControl.HoldType.Foot);
		}
		//Debug.Log(type + " " + name);
	}
}
