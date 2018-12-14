using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HScene2{
	private string[] onHolds;
	private List<string> comments;
	private Vector3[] pose;
	private Quaternion[] pRotate;
	private bool posable;
	private int id;
	private static int num = 0;
	private bool isSaved;
	//シーンを読み込んでからポーズを保存したかどうか
	private bool isPoseSaved = false;

	public HScene2(){
		onHolds = new string[4];
		comments = new List<string>();
		pose = new Vector3[Enum.GetNames(typeof(AvatarControl.BODYS)).Length - 1];
		pRotate = new Quaternion[Enum.GetNames(typeof(AvatarControl.BODYS)).Length - 1];
		posable = false;
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

	public List<string> GetComments(){
		return new List<string>(comments);
	}

	public void SaveComments(List<string> com){
		comments = new List<string>(com);
		isSaved = true;
	}

	public Vector3[] GetPose(){
		return pose;
	}

	public void SavePose(Vector3[] arr){
		posable = true;
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

	public bool IsPose(){
		return posable;
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

	//hScene間の同値判定
	//pose[], pRotate[], onHolds[]の値が等しい時true
	public bool IsEqualTo(Hold[] aOnHolds, Vector3[] aPose, Quaternion[] aRotate){
		//string[] aOnHolds = another.GetOnHolds();
		for(int i = 0 ; i < onHolds.Length ; i++){
			string another = null;
			if (aOnHolds[i] != null){
				another = aOnHolds[i].gameObject.name;
			}
			//Debug.Log("compare "+ onHolds[i] +" and "+ another);
			if (!String.Equals(onHolds[i], another)){
				//Debug.Log("false in onHolds");
				return false;
			}
		}

		//Vector3[] aPose = another.GetPose();
		for(int i = 0 ; i < pose.Length ; i++){
			if (aPose[i] != pose[i]){
				//Debug.Log("false in pose");
				return false;
			}
		}

		//Quaternion[] aRotate = another.GetPRotate();
		for(int i = 0 ; i < pRotate.Length ; i++){
			//Debug.Log("ratate " + pRotate[i].eulerAngles + " and "+ aRotate[i].eulerAngles);
			if (pRotate[i] != aRotate[i]){
				//Debug.Log("false in rotate");
				return false;
			}
		}

		return true;
	}
}
