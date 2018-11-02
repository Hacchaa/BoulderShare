using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HScene{
	private string[] onHolds;
	private List<string> comments;
	private Vector3[] pose;
	private Quaternion[] pRotate;
	private bool posable;

	public HScene(){
		onHolds = new string[4];
		comments = new List<string>();
		pose = new Vector3[Enum.GetNames(typeof(AvatarControl.BODYS)).Length];
		pRotate = new Quaternion[Enum.GetNames(typeof(AvatarControl.BODYS)).Length];
		posable = false;
	}
	
	public string[] GetOnHolds(){
		return onHolds;
	}
	public void SetOnHolds(String[] h){
		onHolds = h;
	}

	public void SaveOnHolds(Hold[] holds){
		for(int i = 0 ; i < onHolds.Length ; i++){
			if (holds[i] != null){
				onHolds[i] = holds[i].gameObject.name;
			}else{
				onHolds[i] = null;
			}
		}
	}

	public List<string> GetComments(){
		return new List<string>(comments);
	}

	public void SaveComments(List<string> com){
		comments = new List<string>(com);
	}

	public Vector3[] GetPose(){
		return pose;
	}

	public void SavePose(Vector3[] arr){
		posable = true;
		pose = arr;
	}

	public Quaternion[] GetPRotate(){
		return pRotate;
	}

	public void SavePRotate(Quaternion[] arr){
		pRotate = arr;
	}

	public bool IsPose(){
		return posable;
	}
	
	public void show(){
		for(int i = 0 ; i < onHolds.Length ; i++){
			if (onHolds[i] != null){
				Debug.Log("onHolds["+i+"]:"+onHolds[i]);
			}
		}
		for(int i = 0 ; i < pose.Length ; i++){
			Debug.Log("pose["+i+"]:"+pose[i]);
		}
		for(int i = 0 ; i < pRotate.Length ; i++){
			Debug.Log("pROtate["+i+"]:"+pRotate[i]);
		}
	}
}
