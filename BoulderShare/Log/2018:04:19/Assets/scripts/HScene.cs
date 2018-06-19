using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HScene{
	private string[] onHolds;
	private string comment;

	public HScene(){
		onHolds = new string[4];
	}
	
	public string[] GetOnHolds(){
		return onHolds;
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

	public string GetComment(){
		return comment;
	}

	public void SaveComment(string com){
		comment = com;
	}
	
	public void show(){
		for(int i = 0 ; i < onHolds.Length ; i++){
			if (onHolds[i] != null){
				Debug.Log(i+":"+onHolds[i]);
			}
		}
	}
}
