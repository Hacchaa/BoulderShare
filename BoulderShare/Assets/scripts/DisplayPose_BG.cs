using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayPose_BG : MonoBehaviour, IPointerClickHandler {
	public DisplayPoseOperation op;
	// Use this for initialization
	void Start () {
		
	}
	
	public void OnPointerClick(PointerEventData data){
		op.Open();
	}
}
