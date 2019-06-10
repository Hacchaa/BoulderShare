using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class TwoDWallMarks : MonoBehaviour {
	private TwoDMark current = null;
	private int index = 0;
	private TwoDMark[] touchMarks;
	private TwoDMark[] dummyMarks;
	private static int num = 0;
	private Dictionary<string, TwoDMark> map;
	[SerializeField] private GameObject twoDMarkOrigin;
	[SerializeField] private WallManager wallManager;
	private Action focusOnAction;
	private Action focusOffAction;
	private bool isIgnoreAction;

	void Awake(){
		touchMarks = new TwoDMark[4];
		dummyMarks = new TwoDMark[4];
		map = new Dictionary<string, TwoDMark>();
	}

	private void Init(){
		map.Clear();
		DeleteMarks();
		for(int i = 0 ; i < touchMarks.Length ; i++){
			touchMarks[i] = null;
			dummyMarks[i] = null;
		}
		current = null;
		index = 0;
	}

	public static int GetNum(){
		return num;
	}
	public static void SetNum(int n){
		num = n;
	}
	public GameObject GetWallMarks(){
		return this.gameObject;
	}
	public void DeleteMarks(){
		foreach(Transform child in transform){
			Destroy(child.gameObject);
		}
	}

	public bool DeleteMark(string name){
		if (map.ContainsKey(name)){
			ReleaseFocus();
			GameObject obj = map[name].gameObject;
			bool b = map.Remove(name);
			Destroy(obj.gameObject);
			return b;
		}
		return false;
	}

	public GameObject MakeMark(string name = ""){
		GameObject obj = Instantiate(twoDMarkOrigin, transform);
		obj.SetActive(true);
		if (string.IsNullOrEmpty(name)){
			obj.name = num + "";
		}else{
			obj.name = name;
		}
		map.Add(obj.name, obj.GetComponent<TwoDMark>());

		num++;

		return obj;
	}

	public bool IsMarkExist(string name){
		return map.ContainsKey(name);
	}

	public bool IsMarkExist(){
		return transform.childCount > 0 ;
	}

	public void SetFocusOnAction(Action action){
		if (action != null){
			focusOnAction += action;
		}
	}

	public void SetFocusOffAction(Action action){
		if (action != null){
			focusOffAction += action;
		}
	}

	public void ResetFocusOnAction(){
		focusOnAction = null;
	}
	public void ResetFocusOffAction(){
		focusOffAction = null;
	}

	public void IgnoreFocusAction(){
		isIgnoreAction = true;
	}
	public void AcceptFocusAction(){
		isIgnoreAction = false;
	}

	public void SetFocus(TwoDMark mark){
		if (current != null){
			current.ReleaseFocus();
		}

		current = mark;
		current.Focus();
		if (!isIgnoreAction && focusOnAction != null){
			focusOnAction();
		}
	}

	public TwoDMark GetFocus(){
		return current;
	}

	public void ReleaseFocus(){
		if (current != null){
			current.ReleaseFocus();
			if (!isIgnoreAction && focusOffAction != null){
				focusOffAction();
			}
		}
		current = null;
	}

	public void Next(){
		int n = transform.childCount;
		if (n == 0){
			return ;
		}
		index++;
		if (index > n - 1){
			index = 0;
		}
		SetFocus(transform.GetChild(index).GetComponent<TwoDMark>());
	}

	public void Prev(){
		int n = transform.childCount;
		if (n == 0){
			return ;
		}
		index--;
		if (index < 0){
			index = n - 1;
		}
		SetFocus(transform.GetChild(index).GetComponent<TwoDMark>());
	}

	public void IgnoreEvents(){
		foreach(Transform child in transform){
			child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
	}

	public void AcceptEvents(){
		foreach(Transform child in transform){
			child.gameObject.layer = LayerMask.NameToLayer("2D");
		}
	}

	public void AcceptEvents(List<string> list){
		if (list == null){
			return ;
		}
		foreach(string id in list){
			if (map.ContainsKey(id)){
				map[id].gameObject.layer = LayerMask.NameToLayer("2D");
			}			
		}
	}

	public void MakeMarksActive(){
		foreach(TwoDMark mark in map.Values){
			mark.ActivateNoActiveObj(false);
		}		
	}

	public void MakeMarksActive(List<string> list){
		if (list == null){
			return ;
		}
		foreach(string id in list){
			if (map.ContainsKey(id)){
				map[id].ActivateNoActiveObj(false);
			}			
		}
	}

	public void MakeMarksNoActive(){
		foreach(TwoDMark mark in map.Values){
			mark.ActivateNoActiveObj(true);
		}	
	}

	public TwoDMark CalcNearestMark(Vector2 v){
		float min = float.MaxValue;
		TwoDMark mark = null;
		foreach(Transform child in transform){
			float tmp = (v - new Vector2(child.position.x, child.position.y)).magnitude;
			if (tmp < min){
				min = tmp;
				mark = child.GetComponent<TwoDMark>();
			}
		}

		return mark;
	}

	public void Touch(TwoDMark mark, TwoDMark.HFType bodyType){
		if (mark == touchMarks[(int)bodyType]){
			mark.SetTouchInfo(bodyType, false);
			touchMarks[(int)bodyType] = null;
			return ;
		}

		if (touchMarks[(int)bodyType] != null){
			touchMarks[(int)bodyType].SetTouchInfo(bodyType, false);
			touchMarks[(int)bodyType] = null;
		}

		mark.SetTouchInfo(bodyType, true);
		touchMarks[(int)bodyType] = mark;
	}

	public void ClearTouch(){
		for(int i = 0 ; i < touchMarks.Length ; i++){
			if (touchMarks[i] != null){
				touchMarks[i].Clear();
				touchMarks[i] = null;
			}
			if (dummyMarks[i] != null){
				dummyMarks[i].Clear();
				dummyMarks[i] = null;
			}
		}
	}

	public string[] GetTouchInfo(){
		string[] arr = new string[4];

		for(int i = 0 ; i < touchMarks.Length ; i++){
			if(touchMarks[i] == null){
				arr[i] = null;
			}else{
				arr[i] = touchMarks[i].gameObject.name;
			}
		}

		return arr;
	}

	public void SetTouchInfo(string[] arr){
		for(int i = 0 ; i < touchMarks.Length ; i++){
			//Debug.Log("set:"+arr[i]);
			if (!string.IsNullOrEmpty(arr[i]) && IsMarkExist(arr[i])){
				touchMarks[i] = map[arr[i]];
				touchMarks[i].SetTouchInfo((TwoDMark.HFType)i, true);
			}
		}
	}

	public void SetDummyTouchInfo(string[] arr){
		for(int i = 0 ; i < dummyMarks.Length ; i++){
			if (!string.IsNullOrEmpty(arr[i]) && IsMarkExist(arr[i])){
				dummyMarks[i] = map[arr[i]];
				dummyMarks[i].SetDummyTouchInfo((TwoDMark.HFType)i, true);
			}
		}
	}

	public void Synchronize(GameObject rootMarks){
		Init();
		foreach(Transform t in rootMarks.transform){
			if(!string.IsNullOrEmpty(t.name)){
				GameObject mark = MakeMark(t.name);
				mark.transform.localPosition = t.localPosition;
				mark.transform.localScale = t.localScale;
				mark.SetActive(true);
				map[t.name].ReleaseFocus();
			}
		}
	}
}
