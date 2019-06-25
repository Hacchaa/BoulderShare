using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScreenTransitionManager : MonoBehaviour {
	private List<SEComponentBase> uiList;
	[SerializeField] private Transform stAnimationRoot;
	[SerializeField] private Canvas uiCanvas;
	private STAnimationBase[,] stAnimations;
	private Dictionary<string, SEComponentBase> map;
	private SEComponentBase current = null;
	private string prev;
	private string curName;
	private Screen prevScreen;
	private Screen curScreen;

	[SerializeField] private CanvasResolutionManager canResManager;
	[SerializeField] private List<RenderTexture> rtList;

	public enum Screen {
		Post,
		ThreeDFirstSettingView,
		MainView,
		LayerGraphView,
		AttemptTreeMenu,
		SceneEditor,
		ModifyMarks,
		EditInfoView,
		InputTextView
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
		foreach(Transform t in uiCanvas.transform){
			SEComponentBase com = t.GetComponent<SEComponentBase>();
			if (com != null && IsScreenVaild(com.name)){
				map.Add(com.name, com);
				com.Hide(false);
				uiList.Add(com);				
			}
		}
		int n = Enum.GetNames(typeof(Screen)).Length;
		stAnimations = new STAnimationBase[n,n];
		CanvasGroup cg = uiCanvas.GetComponent<CanvasGroup>();

		foreach(Transform t in stAnimationRoot){
			STAnimationBase anim = t.GetComponent<STAnimationBase>();
			if (anim != null){
				ScreenTransitionManager.Screen from = anim.GetFrom();
				ScreenTransitionManager.Screen to = anim.GetTo();

				stAnimations[(int)from, (int)to] = anim;

				if (map.ContainsKey(from.ToString()) && map.ContainsKey(to.ToString())){
					anim.Init(map[from.ToString()], map[to.ToString()], cg);
				}
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
	
	public void Transition(Screen screen){
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

			if (!string.IsNullOrEmpty(prev) && stAnimations[(int)prevScreen, (int)curScreen] != null){
				//Debug.Log("animatioN");
				stAnimations[(int)prevScreen, (int)curScreen].Play();
			}else{
				//Debug.Log("no animation");
				current.Show();
				if (!string.IsNullOrEmpty(prev)){
					map[prev].Hide();
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
