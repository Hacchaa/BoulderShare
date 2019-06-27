using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class EditorManager : MonoBehaviour {
	//public enum BODYS{NONE=-1,RH,LH,RF,LF,RE,LE,RK,LK,BODY, LOOK};
	[SerializeField]
	private Color color;
	public static string BOROUTEPATH = "/route/";
	public const string WALLPATH = "/";
	[SerializeField]
	private EditorPopup popup;
	[SerializeField]
	private Post2 post;
	[SerializeField]
	private HumanModel humanModel;
	[SerializeField]
	private string timeStamp ;
	[SerializeField]
	private BorouteLSManager2 bManager;
	[SerializeField]
	private ScreenTransitionManager transition;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private bool isCalcFPS = false;
	private float fps;
	[SerializeField] private float updateInterval = 0.5f;
	private float fpsSum;
	private int frames;
	private float leftTime;
	[SerializeField] private Text fpsText;
	private string test;

	void Awake(){
		timeStamp = "";
		Application.targetFrameRate = 60;
	}

	void Start(){
		FirstProc();
	}
	void Update(){
		if (isCalcFPS){
			leftTime -= Time.deltaTime;
			fpsSum += Time.timeScale / Time.deltaTime;
			frames++;

			if (0 < leftTime){
				return ;
			}
			fps = fpsSum / frames;
			leftTime = updateInterval;
			fpsSum = 0;
			frames = 0;
			fpsText.text = "" + fps;
		}
	}

	public float GetFPS(){
		return fps;
	}

	//borouteeditの初期化処理
	private void FirstProc(){
		hScenes.Init();
		transition.Init();
		transition.Transition(ScreenTransitionManager.Screen.MainView);

		GameObject obj = DontDestroyOnLoadManager.Get("InfoFromViewToEdit");
		if(obj == null){
		}else{
			InfoFromViewToEdit info = obj.GetComponent<InfoFromViewToEdit>();
			SetPList(info.GetPlaceList());
			if (info.IsNew()){
			}else{
				bManager.LoadBoroute();
			}
		}
	}

	public void SetPList(List<string> list){
		post.SetPList(list);
	}

	public void ExitEditor(){
		string title = "終了しますか？";
		string content = "編集内容は一時保存されます。";
		popup.Open(YesProc, null, title, content, "終了", "キャンセル");
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
		boroute.incline = wallManager.GetMasterIncline();
		boroute.grade = post.GetGrade();
		boroute.tryCount = post.GetTryCount();
		boroute.isComplete = post.IsComplete();
		boroute.scaleH2M = humanModel.GetModelSize();
		boroute.globalComment = post.GetGlobalComment();

		return boroute;
	}

	public void LoadBorouteInfo(MyUtility.BorouteInformation boroute){
		timeStamp = boroute.timestamp;
		post.SetDate(boroute.date);
		post.SetPlace(boroute.place);
		wallManager.CommitIncline(boroute.incline);
		post.SetGrade(boroute.grade);
		post.SetTryCount(boroute.tryCount);
		post.SetIsComplete(boroute.isComplete);
		humanModel.SetModelSize(boroute.scaleH2M);
		post.SetGlobalComment(boroute.globalComment);
	}
/*
	public void PrintJson(){
		test = BorouteInfoToJson();
		Debug.Log(test);
	}

	public void apply(){
		BorouteInfoFromJson(test);
	}
	public static string BorouteInfoForSearchingToJson(List<MyUtility.BorouteInformation> list){
		MyUtility.BorouteInfoForSearching b = new MyUtility.BorouteInfoForSearching();
		b.data = new List<MyUtility.BorouteInformation>(list);
		return JsonUtility.ToJson(b);
	}

	public static List<MyUtility.BorouteInformation> BorouteInfoForSearchingFromJson(string json){
		return JsonUtility.FromJson<MyUtility.BorouteInfoForSearching>(json).data;
	}*/
}
