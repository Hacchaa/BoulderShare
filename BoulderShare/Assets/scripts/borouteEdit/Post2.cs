using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Post2 :SEComponentBase{
	[SerializeField] private ScreenTransitionManager stManager;
	[SerializeField] private PlaceDD2 placeDD;
	[SerializeField] private GradeDD2 gradeDD;
	[SerializeField] private DateDD2 dataDD;
	[SerializeField] private Dropdown tryDD;
	[SerializeField] private Toggle completeToggle;
	[SerializeField] private InputField gComment;
	[SerializeField] private ScrollRect sr;
	[SerializeField] private BorouteLSManager2 bManager;
	[SerializeField] private EditorManager eManager;

	public void PostBoroute(){
		bManager.SaveBoroute();
	}

	public void Exit(){
		eManager.ExitImmediately();
	}

	public void Post(){
		PostBoroute();
		Exit();
	}

	public override void OnPreShow(){
		//課題一覧を一番上にスクロールする
		sr.verticalNormalizedPosition = 1.0f;
	}

	public override void OnPreHide(){
	}

	public void ToRouteDetailView(){
		stManager.Transition(ScreenTransitionManager.Screen.RouteDetailView);
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
