using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastBlockerForSceneComment : MonoBehaviour, IPointerClickHandler
{	
	[SerializeField]
	private SceneCommentController scc;

	public void OnPointerClick(PointerEventData data){
		scc.Release();
	}
}
