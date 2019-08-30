using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class ScreenTransitionManager : SingletonMonoBehaviour<ScreenTransitionManager>{
	private List<SEComponentBase> uiList;
	[SerializeField] private Transform bsTransitionRoot;
	[SerializeField] private Canvas uiCanvas;
	private BSScreenTransition[,] bsTransitions;
	private Dictionary<string, SEComponentBase> map;
	private SEComponentBase current = null;
	private string prev;
	private string curName;
	private Screen prevScreen;
	private Screen curScreen;

	[SerializeField] private string[] screenArr;
	private Stack<string> screenStack;

	[SerializeField] private CanvasResolutionManager canResManager;
	[SerializeField] private List<RenderTexture> rtList;

	public enum Screen {
		RoutesView,
		MakeRouteView,
		RouteDetailView,
		AttemptRecordsView,
		MakeAttemptRecordView,
		RouteAnalyticsView,
		SearchRouteView,
		Post,
		ThreeDFirstSettingView,
		LayerGraphView,
		AttemptTreeMenu,
		SceneEditor,
		ModifyMarks,
		EditInfoView,
		InputTextView,
		EditModelFigureView
	}

	void Update(){
		//screenArr = screenStack.ToArray();
	}
/*
	void Awake(){
		map = new Dictionary<string, SEComponentBase>();
		uiList = new List<SEComponentBase>();
		prev = "";
		curName = "";
	}*/

	void Start(){
		/*
		foreach(SEComponentBase obj in uiList){
			SEComponentBase com = obj.GetComponent<SEComponentBase>();
			map.Add(obj.name, com);
			com.Hide();
		}
		Transition("AttemptTreeView");*/
	}

	public List<SEComponentBase> GetUIList(){
		return uiList;
	}

	public void Init(){
		map = new Dictionary<string, SEComponentBase>();
		uiList = new List<SEComponentBase>();
		prev = "";
		curName = "";
		screenStack = new Stack<string>();
		foreach(Transform t in uiCanvas.transform){
			SEComponentBase com = t.GetComponent<SEComponentBase>();
			if (com != null && IsScreenVaild(com.name)){
				map.Add(com.name, com);
				//com.OnPreHide();
				com.HideUI();
				uiList.Add(com);				
			}
		}
		int n = Enum.GetNames(typeof(Screen)).Length;
		bsTransitions = new BSScreenTransition[n,n];
		CanvasGroup cg = uiCanvas.GetComponent<CanvasGroup>();

		foreach(Transform t in bsTransitionRoot){
			BSScreenTransition trans = t.GetComponent<BSScreenTransition>();
			if (trans != null){
				trans.Init();
				ScreenTransitionManager.Screen from = trans.GetFrom();
				ScreenTransitionManager.Screen to = trans.GetTo();

				bsTransitions[(int)from, (int)to] = trans;
/*
				if (map.ContainsKey(from.ToString()) && map.ContainsKey(to.ToString())){
					trans.Init(map[from.ToString()], map[to.ToString()], cg);
				}*/
			}
		}

		//setting renderTexture
		foreach(RenderTexture rt in rtList){
			rt.width = UnityEngine.Screen.width;
			rt.height = UnityEngine.Screen.height;			
		}
	}
	//nameがenum.Screenに含まれているかどうか
	private bool IsScreenVaild(string name){
		foreach(string v in Enum.GetNames(typeof(Screen))){
			if (!string.IsNullOrEmpty(v) && name.Equals(v)){
				return true;
			}
		}
		return false;
	}

	public void InitScreen(Screen screen){
		string name = screen.ToString();
		if (!IsScreenVaild(name)){
			return ;
		}

		if (map.ContainsKey(name)){
			map[name].Show();
		}
	}
	
	public void Transition(Screen screen, bool isNeedInit = true){
		if (isNeedInit){
			InitScreen(screen);
		}
		TransitionByName(screen);
	}

	private void TransitionByName(Screen screen){
		string name = screen.ToString();
		if (map.ContainsKey(name)){
			if (current != null){
				prev = curName;
				prevScreen = curScreen;
			}
			current = map[name];
			curName = name;
			curScreen = screen;

			if (!screenStack.Any()){
				screenStack.Push(name);
			}

			if (!string.IsNullOrEmpty(prev) && bsTransitions[(int)prevScreen, (int)curScreen] != null){
				//Debug.Log("animatioN");
				BSScreenTransition trans = bsTransitions[(int)prevScreen, (int)curScreen];
				trans.UpdateScreenStack(screenStack, false);
				trans.BSTransitionWithAnim();

			}else if(!string.IsNullOrEmpty(prev) && bsTransitions[(int)curScreen, (int)prevScreen] != null){
				BSScreenTransition trans = bsTransitions[(int)curScreen, (int)prevScreen];
				trans.UpdateScreenStack(screenStack, true);
				trans.ReverseBSTransitionWithAnim();
			}else{
				//Debug.Log("no animation");
				current.Show();
				if (!string.IsNullOrEmpty(prev)){
					map[prev].OnPreHide();
					map[prev].HideUI();
				}
			}
		}		
	}

	//ひとつ前限定
	public void Back(){
		if (!string.IsNullOrEmpty(prev)){
			TransitionByName(prevScreen);
			prev = "";
		}
	}
}
