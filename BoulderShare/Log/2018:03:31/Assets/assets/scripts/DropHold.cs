using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHold : MonoBehaviour, IDropHandler {
	public GameObject holds;
	public GameObject[] acceptedObj ;
	public GameObject[] holdPrefabs ;
	private int dIndex;
	// Use this for initialization
	void Start () {
	}

	public void OnDrop(PointerEventData data){
		if (data.pointerDrag == null){
			return ;
		}
		bool isDroppable = false;
		for(int i = 0 ; i < acceptedObj.Length ; i++){
			if (acceptedObj[i] == data.pointerDrag){
				isDroppable = true;
				dIndex = i;
				break;
			}
		}/*
		foreach(GameObject obj in acceptedObj){
			if (obj == data.pointerDrag){
				isDroppable = true;
				break;
			}
		}*/

		if (isDroppable){
			GameObject hold = Instantiate(holdPrefabs[dIndex], Vector2.zero, Quaternion.identity);
			hold.transform.SetParent(holds.transform, false);
			hold.transform.position = data.position;
			/*
			if (hold.GetComponent<CanvasGroup>() != null){
				Destroy(hold.GetComponent<CanvasGroup>());
			}
			hold.name = "hold";*/
		}
	}
}
