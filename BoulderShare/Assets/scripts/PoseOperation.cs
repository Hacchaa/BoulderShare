using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PoseOperation : MonoBehaviour {
	public GameObject pose;
	public GameObject display;
	public IKControl avatarDisplay;
	public IKControl avatarPose;

	// Use this for initialization
	void Start () {
		
	}

	public void Apply(){
		avatarDisplay.SetPose(avatarPose.GetPose());
		display.SetActive(true);
		pose.SetActive(false);	
	}

	public void Close(){
		display.SetActive(true);
		pose.SetActive(false);
	}
}
