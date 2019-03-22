using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScreenTransitionManager : MonoBehaviour {
	[SerializeField]
	private List<SEComponentBase> uiList;
	[SerializeField]
	private Dictionary<string, SEComponentBase> map;
	private SEComponentBase current = null;
	private string prev;
	private string curName;
	[SerializeField] Image topSafeArea;
	[SerializeField] Image botSareArea;

	public enum Screen {
		AttemptTreeView,
		EditScene,
		TryView,
		Post,
		EditPose,
		PickImageView,
		ThreeDFirstSettingView,
		Edit3DSceneComment,
		MainView,
		EditWallMark,
		SceneSelectView,
		LayerGraphView
	}

	void Awake(){
		map = new Dictionary<string, SEComponentBase>();
		prev = "";
		curName = "";
	}

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
		foreach(SEComponentBase com in uiList){
			if (IsScreenVaild(com.name)){
				map.Add(com.name, com);
				com.Hide(false);				
			}
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
		TransitionByName(screen.ToString());
	}

	private void TransitionByName(string name){
		if (map.ContainsKey(name)){
			if (current != null){
				prev = curName;
				current.Hide();
			}
			current = map[name];
			current.Show();
			curName = name;
			topSafeArea.color = current.GetTopSEColor();
			botSareArea.color = current.GetBotSEColor();
		}		
	}

	//ひとつ前限定
	public void Back(){
		if (!string.IsNullOrEmpty(prev)){
			TransitionByName(prev);
			prev = "";
		}
	}
}
