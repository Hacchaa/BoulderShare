using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Kakera;

public class EditorManager : MonoBehaviour {
	public enum BODYS{NONE=-1,RH,LH,RF,LF,RE,LE,RK,LK,BODY};
	public static string BOROUTEPATH = "/route/";
	public const string WALLPATH = "/";
	[SerializeField]
	private EditorPopup popup;
	[SerializeField]
	private TwoDWall twoDWall;
	[SerializeField]
	private ThreeDWall threeDWall;
	[SerializeField]
	private Post2 post;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private string timeStamp ;
	[SerializeField]
	private PickerController2 pc;
	[SerializeField]
	private BorouteLSManager2 bManager;
	[SerializeField]
	private ScreenTransitionManager transition;

	private string test;

	void Awake(){
		timeStamp = "";
	}

	void Start(){
		GameObject obj = DontDestroyOnLoadManager.Get("InfoFromViewToEdit");
		if(obj == null){
			pc.OpenImagePicker();
			return ;
		}

		InfoFromViewToEdit info = obj.GetComponent<InfoFromViewToEdit>();
		if (info.IsNew()){
			pc.OpenImagePicker();
		}else{
			bManager.LoadBoroute();
		}
		SetPList(info.GetPlaceList());
		transition.Transition("AttemptTreeView");
	}

	public void SetPList(List<string> list){
		post.SetPList(list);
	}

	public void ExitEditor(){
		string content = "編集作業を一時保存しますか？";
		popup.Open(YesProc, ExitImmediately, content);
	}

	private void YesProc(){
		bManager.SaveBoroute(true);
		ExitImmediately();
	}
	public void ExitImmediately(){
		SceneManager.LoadScene("routeview2");
	}

	public string GetTimestamp(){
		return timeStamp;
	}

	public MyUtility.BorouteInformation GetBorouteInfo(){
		MyUtility.BorouteInformation boroute = new MyUtility.BorouteInformation();

		//ボルートが新規作成でない場合、タイムスタンプは更新しない
		if (string.IsNullOrEmpty(timeStamp)){
			boroute.timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
			timeStamp = boroute.timestamp;
		}else{
			boroute.timestamp = timeStamp;
		}
		boroute.date = post.GetDate();
		boroute.place = post.GetPlace();
		boroute.incline = threeD.GetWallIncline();
		boroute.grade = post.GetGrade();
		boroute.tryCount = post.GetTryCount();
		boroute.isComplete = post.IsComplete();
		boroute.scaleH2M = threeD.GetModelSize();
		boroute.globalComment = post.GetGlobalComment();

		return boroute;
	}

	public string BorouteInfoToJson(){
		return JsonUtility.ToJson(GetBorouteInfo());
	}

	public void BorouteInfoFromJson(string json){
		MyUtility.BorouteInformation boroute = JsonUtility.FromJson<MyUtility.BorouteInformation>(json);

		timeStamp = boroute.timestamp;
		post.SetDate(boroute.date);
		post.SetPlace(boroute.place);
		threeD.SetWallIncline(boroute.incline);
		post.SetGrade(boroute.grade);
		post.SetTryCount(boroute.tryCount);
		post.SetIsComplete(boroute.isComplete);
		threeD.SetModelSize(boroute.scaleH2M);
		post.SetGlobalComment(boroute.globalComment);
	}

	public void PrintJson(){
		test = BorouteInfoToJson();
		Debug.Log(test);
	}

	public void apply(){
		BorouteInfoFromJson(test);
	}/*
	public static string BorouteInfoForSearchingToJson(List<MyUtility.BorouteInformation> list){
		MyUtility.BorouteInfoForSearching b = new MyUtility.BorouteInfoForSearching();
		b.data = new List<MyUtility.BorouteInformation>(list);
		return JsonUtility.ToJson(b);
	}

	public static List<MyUtility.BorouteInformation> BorouteInfoForSearchingFromJson(string json){
		return JsonUtility.FromJson<MyUtility.BorouteInfoForSearching>(json).data;
	}*/
}
