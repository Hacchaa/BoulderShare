using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hold : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private static int finger = -1;
	private Camera curCamera;
	private Observer observer;
	private GameObject child;
	private Renderer rend;
	private Vector3 oldP;
	public RectTransform scalePos;
	public static GameObject selected = null;
	private RectTransform canvasPos;
	private static float R = 0.5f;

	// Use this for initialization
	void Start () {
		GameObject tmp = GameObject.Find("Observer");
		observer = tmp.GetComponent<Observer>();
		curCamera = observer.GetCamera();
		child = transform.Find("Hold_Scale").gameObject;

		rend = GetComponent<Renderer>();
		canvasPos = GameObject.Find("Canvas").GetComponent<RectTransform>();
		scalePos = GameObject.Find("Canvas").transform.Find("Focus").gameObject.GetComponent<RectTransform>();
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == -1){
			finger = data.pointerId;
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			rend.sortingLayerName = "Hold";
			observer.ReleaseFocus();
			selected = gameObject;

			oldP = curCamera.ScreenToWorldPoint(
					new Vector3(
						data.position.x, 
						data.position.y, 
						-curCamera.transform.position.z));
		}
	}

	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = curCamera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-curCamera.transform.position.z));
			
	        transform.Translate(p - oldP);

	        oldP = p;
	    }
	}
	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = -1;
		

			//bounds
			float width = Observer.WALL_W;
			float height = Observer.WALL_H;
			Vector3 p = transform.position;

			p.x = Mathf.Min(p.x, width/2);
	    	p.x = Mathf.Max(p.x, -width/2);

	    	p.y = Mathf.Min(p.y, height/2);
	    	p.y = Mathf.Max(p.y, -height/2);

	    	transform.position = p;

			SetScalePos();
			gameObject.layer = LayerMask.NameToLayer("Hold");
			rend.sortingLayerName = "Defalut";
		}
	}

	public void OnPointerClick(PointerEventData data){
		SetScalePos();
		selected = gameObject;
	}

	public static GameObject GetSelectedHold(){
		return selected;
	}
	
	public Vector2 WorldToCanvasPoint(Vector3 position){
		Vector2 pos;
		Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvasPos,
			screenPos,
			curCamera,
			out pos);
		
		return pos;
	}

	public void SetScalePos(){
		
		float rate = transform.localScale.x;
		scalePos.localPosition = WorldToCanvasPoint(new Vector3(
				transform.position.x + R * rate,
				transform.position.y - R * rate,
				transform.position.z));
		observer.FocusObject(scalePos.gameObject);
		//observer.FocusObject(child);
	}
}
