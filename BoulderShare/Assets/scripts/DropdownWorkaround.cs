using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownWorkaround : MonoBehaviour {

	public string sortingLayerName = "Default";

	void Awake(){
		Canvas canvas = GetComponent<Canvas>();
		if (canvas != null){
			canvas.sortingLayerName = sortingLayerName;
		}
	}
}
