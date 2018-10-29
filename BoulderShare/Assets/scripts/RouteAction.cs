using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteAction : MonoBehaviour {

	public void Test(){
		Debug.Log("hi");
	}

	//phase1 → phase2
	public void Switch(){
		Observer obs = GameObject.Find("Observer").GetComponent<Observer>();
		obs.SwitchPhase(1);
	}
}