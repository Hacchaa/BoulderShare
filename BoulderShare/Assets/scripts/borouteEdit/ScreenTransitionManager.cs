using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionManager : MonoBehaviour {
	[SerializeField]
	private List<GameObject> uiList;
	[SerializeField]
	private Dictionary<string, IUIComponent> map;
	private IUIComponent current = null;

	void Awake(){
		map = new Dictionary<string, IUIComponent>();
	}

	void Start(){
		foreach(GameObject obj in uiList){
			IUIComponent com = obj.GetComponent<IUIComponent>();
			map.Add(obj.name, com);
			com.Hide();
		}
		Transition("AttemptTreeView");
	}
	
	public void Transition(string name){
		if (map.ContainsKey(name)){
			if (current != null){
				current.HideProc();
			}
			current = map[name];
			current.ShowProc();
		}
	}
}
