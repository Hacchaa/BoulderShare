using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayPoseOperation : MonoBehaviour{
	public GameObject pose;
	public GameObject display;
	public IKControl avatarDisplay;
	public IKControl avatarPose;
	public Hold_Pose hp;
	// Use this for initialization
	void Start () {
		
	}
	
	public void Open(){
		hp.Sync();
		avatarPose.SetPose(avatarDisplay.GetPose());
		pose.SetActive(true);
		display.SetActive(false);
	}
}
