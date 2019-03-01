using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
			//SEComponentBase com = com.GetComponent<SEComponentBase>();
			map.Add(com.name, com);
			com.Hide();
		}
	}
	
	public void Transition(string name){
		if (map.ContainsKey(name)){
			if (current != null){
				prev = curName;
				current.HideProc();
			}
			current = map[name];
			current.ShowProc();
			curName = name;
			topSafeArea.color = current.GetTopSEColor();
			botSareArea.color = current.GetBotSEColor();
		}
	}

	//ひとつ前限定
	public void Back(){
		if (!string.IsNullOrEmpty(prev)){
			Transition(prev);
			prev = "";
		}
	}
}
