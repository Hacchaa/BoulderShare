using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HScene2{
	private string[] onHolds;
	private List<MyUtility.SceneCommentData> comments;
	private Vector3[] pose;
	private Quaternion[] pRotate;
	private bool isLookingActivate ;
	private int id;
	private static int num = 0;
	private bool isSaved;
	//シーンを読み込んでからポーズを保存したかどうか
	private bool isPoseSaved = false;

	public HScene2(){
		onHolds = new string[4];
		comments = new List<MyUtility.SceneCommentData>();
		pose = new Vector3[Enum.GetNames(typeof(EditorManager.BODYS)).Length - 1];
		pRotate = new Quaternion[Enum.GetNames(typeof(EditorManager.BODYS)).Length - 1];
		isLookingActivate = false;
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
		return onHolds;
	}
	public void SetOnHolds(String[] h){
		onHolds = h;
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

	public List<MyUtility.SceneCommentData> GetComments(){
		return new List<MyUtility.SceneCommentData>(comments);
	}

	public void SaveComments(List<MyUtility.SceneCommentData> com){
		comments = new List<MyUtility.SceneCommentData>(com);
		isSaved = true;
	}

	public Vector3[] GetPose(){
		return pose;
	}

	public void SavePose(Vector3[] arr){
		pose = arr;
		isSaved = true;
	}

	public Quaternion[] GetPRotate(){
		return pRotate;
	}

	public void SavePRotate(Quaternion[] arr){
		pRotate = arr;
		isSaved = true;
	}

	public bool IsLookingActivate(){
		return isLookingActivate;
	}

	public void SetIsLookingActivate(bool b){
		isLookingActivate = b;
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
		for(int i = 0 ; i < pRotate.Length ; i++){
			Debug.Log("pROtate["+i+"]:"+pRotate[i]);
		}
	}
}
