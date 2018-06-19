using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneNum : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler{
	private SceneNumIcon current;
	private Transform sRoot;
	public GameObject IconPrefab;
	private float startX ;
	private int finger ;
	private static float THRESHOLD = 40.0f;
	private static int num = 0;
	public HScenes hScenes;
	// Use this for initialization
	void Start () {
		sRoot = transform.Find("Items");
		current = null;
		Add(0);
		finger = -1;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == -1){
			startX = data.position.x;
			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			float curX = data.position.x;

			if (curX - startX > THRESHOLD){
				hScenes.NextScene();
				startX = curX;
			}else if (curX - startX < -THRESHOLD){
				hScenes.PrevScene();
				startX = curX;
			}
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			startX = 0.0f;
			finger = -1;
		}
	}
	
	public void Add(int index){
		if (current != null){
			current.DeSelect();
		}
		current = Instantiate(IconPrefab, sRoot).GetComponent<SceneNumIcon>();
		current.gameObject.name = num + "";
		num++;
		current.gameObject.transform.SetSiblingIndex(index);
		current.Select();
	}

	public void Remove(int index){
		int next = -1;
		if (index == current.gameObject.transform.GetSiblingIndex()){
			current = null;
			if (sRoot.childCount > 1){
				if (index == 0){
					next = 0;
				}else{
					next = index - 1;
				}
			}
		}

		GameObject.DestroyImmediate(sRoot.GetChild(index).gameObject);

		if (next != -1){
			current = sRoot.GetChild(next).gameObject.GetComponent<SceneNumIcon>();
			current.Select();
		}
	}

	public void Next(){
		int next = current.gameObject.transform.GetSiblingIndex() + 1;

		if (next < sRoot.childCount){
			current.DeSelect();

			current = sRoot.GetChild(next).gameObject.GetComponent<SceneNumIcon>();
			current.Select();
		}
	}

	public void Prev(){
		int next = current.gameObject.transform.GetSiblingIndex() - 1;

		if (next >= 0){
			current.DeSelect();

			current = sRoot.GetChild(next).gameObject.GetComponent<SceneNumIcon>();
			current.Select();
		}
	}

	public void RemoveAll(){
		Start();
		foreach (Transform child in sRoot){
			GameObject.Destroy(child.gameObject);
		}	
	}

}
