using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HScene2{
	private string[] onHolds;
	private List<MyUtility.SceneCommentData3D> comments;
	private Vector3[] pose;
	private Quaternion[] rots;
	private List<string> failureList;
	private int id;
	private static int num = 0;
	private bool isSaved;
	//シーンを読み込んでからポーズを保存したかどうか
	private bool isPoseSaved = false;

	public HScene2(){
		onHolds = new string[4];
		comments = new List<MyUtility.SceneCommentData3D>();
		pose = new Vector3[Enum.GetNames(typeof(MyUtility.FullBodyMark)).Length];
		rots = new Quaternion[Enum.GetNames(typeof(MyUtility.FullBodyMark)).Length];
		failureList = new List<string>();
		isSaved = false;
		id = num;
		num++;
	}

	public static void SetNum(int n){
		num = n;
	}

	public static int GetNum(){
		return num;
	}

	public void SetFailureList(List<string> list){
		failureList = new List<string>(list);
	}

	public List<string> GetFailureList(){
		return new List<string>(failureList);
	}

	public void AddFailureComment(string str){
		failureList.Add(str);
	}

	public bool IsPoseSaved(){
		return isPoseSaved;
	}

	public void SetIsPoseSaved(bool b){
		isPoseSaved = b;
	}

	//一回以上値が入力されたかどうか
	public bool IsAlreadySaved(){
		return isSaved;
	}

	public int GetID(){
		return id;
	}

	public void SetID(int i){
		id = i;
		isSaved = true;
	}

	public string[] GetOnHolds(){
		string[] copy = new string[onHolds.Length];
		Array.Copy(onHolds, copy, onHolds.Length);
		return copy;
	}
	public void SetOnHolds(String[] h){
		Array.Copy(h, onHolds, onHolds.Length);
		isSaved = true;
	}

	public void SaveOnHolds(Hold[] holds){
		for(int i = 0 ; i < onHolds.Length ; i++){
			if (holds[i] != null){
				onHolds[i] = holds[i].gameObject.name;
			}else{
				onHolds[i] = null;
			}
		}
		isSaved = true;
	}

	public List<MyUtility.SceneCommentData3D> GetComments(){
		return new List<MyUtility.SceneCommentData3D>(comments);
	}

	public void SaveComments(List<MyUtility.SceneCommentData3D> com){
		comments = new List<MyUtility.SceneCommentData3D>(com);
		isSaved = true;
	}

	public Vector3[] GetPose(){
		Vector3[] v = new Vector3[pose.Length];
		Array.Copy(pose, v, pose.Length);
		return v;
	}

	public Quaternion[] GetRots(){
		Quaternion[] r = new Quaternion[rots.Length];
		Array.Copy(rots, r, rots.Length);
		return r;
	}

	public void SavePose(Vector3[] arr, Quaternion[] r){
		Array.Copy(arr, pose, pose.Length);
		Array.Copy(r, rots, rots.Length);
		isSaved = true;
	}
	
	public void Show(){
		for(int i = 0 ; i < onHolds.Length ; i++){
			if (onHolds[i] != null){
				Debug.Log("onHolds["+i+"]:"+onHolds[i]);
			}
		}
		for(int i = 0 ; i < pose.Length ; i++){
			Debug.Log("pose["+i+"]:"+pose[i]);
		}
	}
}
