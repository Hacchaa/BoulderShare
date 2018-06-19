using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class GenerateHold : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
	public GameObject holdPrefab;
	private RectTransform rectTransform;
	private GameObject hold;
	private Canvas canvas;
	// Use this for initialization
	void Start () {
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
	}
	
	public void OnPointerDown(PointerEventData data){
		Debug.Log("hy");
		hold = Instantiate(holdPrefab, Vector2.zero, Quaternion.identity);
		hold.name = "dragging";
		CanvasGroup cg = hold.AddComponent<CanvasGroup>();
		cg.blocksRaycasts = false;
		//hold.transform.SetParent(canvas.transform.Find("Wall").Find("Holds"), false);
		hold.transform.SetParent(canvas.transform, false);

		hold.transform.position = data.position;
	}
	public void OnDrag(PointerEventData data){
		hold.transform.position += new Vector3(data.delta.x, data.delta.y);
	}
	public void OnPointerUp(PointerEventData data){
		Destroy(hold);
	}
}
