using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HVisible : MonoBehaviour {
	public GameObject holds;
	private bool visible = true;
	// Use this for initialization
	void Start () {
		
	}
	
	public void Switch(){
		visible = !visible;
		holds.SetActive(visible);
	}
}
