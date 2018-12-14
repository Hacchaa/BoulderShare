﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Post2 : MonoBehaviour , IUIComponent{
	[SerializeField]
	private ScreenTransitionManager stManager;
	[SerializeField]
	private PlaceDD2 placeDD;
	[SerializeField]
	private GradeDD2 gradeDD;
	[SerializeField]
	private DateDD2 dataDD;
	[SerializeField]
	private Dropdown tryDD;
	[SerializeField]
	private Toggle completeToggle;
	[SerializeField]
	private InputField gComment;

	public void ShowProc(){
		gameObject.SetActive(true);
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		gameObject.SetActive(false);
	}

	public void ToATV(){
		stManager.Transition("AttemptTreeView");
	}

	public string GetPlace(){
		return placeDD.GetPlace();
	}

	public int GetGrade(){
		return gradeDD.GetGrade();
	}

	public string GetDate(){
		return dataDD.GetDate();
	}

	public int GetTryCount(){
		return tryDD.value;
	}

	public bool IsComplete(){
		return completeToggle.isOn;
	}

	public string GetGlobalComment(){
		return gComment.text;
	}

	public void SetPlace(string str){
		placeDD.SetPlace(str);
	}

	public void SetPList(List<string> list){
		if (list != null && list.Any()){
			placeDD.CreateOptions(list);
		}
	}

	public void SetGrade(int grade){
		 gradeDD.SetGrade(grade);
	}

	public void SetDate(string str){
		dataDD.SetDate(str);
	}

	public void SetTryCount(int v){
		tryDD.value = v;
	}

	public void  SetIsComplete(bool b){
		completeToggle.isOn = b;
	}

	public void SetGlobalComment(string str){
		gComment.text = str;
	}
}
