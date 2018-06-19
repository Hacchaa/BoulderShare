using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private static int finger = -1;
	private Camera curCamera;
	private Observer observer;
	private GameObject child;
	private Renderer rend;
	private Vector3 oldP;
	private RectTransform scalePos;
	public static GameObject selected = null;
	private RectTransform canvasPos;
	private static float R = 0.5f;
	private bool[] onHold;
	private SceneFocus sf;
	private Hold holdScript;
	private GameObject[] body;

	// Use this for initialization
	void Start () {
		onHold = new bool[4];
		GameObject tmp = GameObject.Find("Observer");
		observer = tmp.GetComponent<Observer>();
		curCamera = observer.GetCamera();
		child = transform.Find("Hold_Scale").gameObject;

		rend = GetComponent<Renderer>();
		canvasPos = GameObject.Find("Canvas").GetComponent<RectTransform>();
		scalePos = GameObject.Find("Canvas").transform.Find("Phase1").Find("Focus").gameObject.GetComponent<RectTransform>();
		sf = GameObject.Find("Canvas").transform.Find("Phase2").Find("Focus").gameObject.GetComponent<SceneFocus>();
		holdScript = GetComponent<Hold>();

		body = new GameObject[4];
		if (gameObject.tag == "Hold_Normal"){
			body[0] = transform.Find("Phase2").Find("body_RH").gameObject;
			body[1] = transform.Find("Phase2").Find("body_LH").gameObject;
		}
		body[2] = transform.Find("Phase2").Find("body_RF").gameObject;
		body[3] = transform.Find("Phase2").Find("body_LF").gameObject;
	}

	public bool[] GetOnHold(){
		return onHold;
	}

	public void SetOnHold(int index, bool isOn){
		onHold[index] = isOn;
		body[index].SetActive(isOn);
		//Debug.Log("index:"+index +" isOn:"+isOn);
	}

	public void OnPointerDown(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.SCENE_EDIT){
			RectTransform focus = GameObject.Find("Canvas").transform.Find("Phase2").Find("Focus").gameObject.GetComponent<RectTransform>();;
			focus.localPosition = WorldToCanvasPoint(transform.position);
			observer.FocusObject(focus.gameObject);
			sf.LoadOnHold(holdScript);
		}
	}

	public void OnPointerUp(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.SCENE_EDIT){
			int choice = sf.GetChoice();
			//Debug.Log(choice+ " was selected");
			sf.Registration(holdScript);
			observer.ReleaseFocus();

			if (choice == (int)SceneFocus.Choice.RH){

			}else if (choice == (int)SceneFocus.Choice.LH){

			}else if (choice == (int)SceneFocus.Choice.RF){

			}else if (choice == (int)SceneFocus.Choice.LF){
	
			}
		}
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.HOLD_EDIT){
			if (finger == -1){
				finger = data.pointerId;
				gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				rend.sortingLayerName = "Hold";
				observer.ReleaseFocus();
				selected = gameObject;
	/*
				oldP = curCamera.ScreenToWorldPoint(
						new Vector3(
							data.position.x, 
							data.position.y, 
							-curCamera.transform.position.z));*/
			}
		}
	}

	public void OnDrag(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.HOLD_EDIT){
			if (data.pointerId == finger){
				Vector3 p = curCamera.ScreenToWorldPoint(
					new Vector3(
						data.position.x, 
						data.position.y, 
						-curCamera.transform.position.z));
				
				oldP = curCamera.ScreenToWorldPoint(
						new Vector3(
							data.position.x - data.delta.x, 
							data.position.y - data.delta.y, 
							-curCamera.transform.position.z));

		        transform.Translate(p - oldP);

		        //oldP = p;
		    }
		}
	}
	public void OnEndDrag(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.HOLD_EDIT){
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
	}

	public void OnPointerClick(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.HOLD_EDIT){
			SetScalePos();
			selected = gameObject;
		}
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
		/*
		float rate = transform.localScale.x;
		scalePos.localPosition = WorldToCanvasPoint(new Vector3(
				transform.position.x + R * rate,
				transform.position.y - R * rate,
				transform.position.z));
		observer.FocusObject(scalePos.gameObject);*/
		observer.FocusObject(child);
	}
}
