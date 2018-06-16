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
	private RectTransform canvasPos;
	private SceneFocus sf;
	private Hold holdScript;
	private GameObject[] body;
	private RectTransform focus;

	// Use this for initialization
	void Awake () {
		GameObject tmp = GameObject.Find("Observer");
		observer = tmp.GetComponent<Observer>();
		curCamera = observer.GetCamera();
		child = transform.Find("Hold_Scale").gameObject;
		rend = GetComponent<Renderer>();
		holdScript = GetComponent<Hold>();
		finger = -1;

		body = new GameObject[4];
		if (gameObject.tag == "Hold_Normal"){
			body[0] = transform.Find("Phase2").Find("body_RH").gameObject;
			body[1] = transform.Find("Phase2").Find("body_LH").gameObject;
		}
		body[2] = transform.Find("Phase2").Find("body_RF").gameObject;
		body[3] = transform.Find("Phase2").Find("body_LF").gameObject;
	}

	public void SetBodyActive(int index, bool isActive){
		body[index].SetActive(isActive);
	}

	public void OnPointerDown(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.SCENE_EDIT){
			if (canvasPos == null){
				canvasPos = GameObject.Find("Phase2").transform.Find("CanvasP2")
					.GetComponent<RectTransform>();
				focus = canvasPos.Find("Focus").GetComponent<RectTransform>();
				sf = focus.GetComponent<SceneFocus>();
			}
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
				SetSLN("Hold");
				observer.ReleaseFocus();
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
				
				Vector3 oldP = curCamera.ScreenToWorldPoint(
						new Vector3(
							data.position.x - data.delta.x, 
							data.position.y - data.delta.y, 
							-curCamera.transform.position.z));

		        transform.Translate(p - oldP);
		    }
		}
	}
	public void OnEndDrag(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.HOLD_EDIT){
			if (data.pointerId == finger){
				finger = -1;
			
				//bounds
				Vector3 p = transform.position;

				p.x = Mathf.Min(p.x, Observer.WALL_W / 2);
		    	p.x = Mathf.Max(p.x, -Observer.WALL_W / 2);
		    	p.y = Mathf.Min(p.y, Observer.WALL_H / 2);
		    	p.y = Mathf.Max(p.y, -Observer.WALL_H / 2);

		    	transform.position = p;

				Focus_P1();
				gameObject.layer = LayerMask.NameToLayer("Hold");
				SetSLN("Defalut");
			}
		}
	}

	public void OnPointerClick(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.HOLD_EDIT){
			Focus_P1();
		}
	}
	
	public Vector2 WorldToCanvasPoint(Vector3 position){
		Vector2 pos;
		Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, position);

		if (canvasPos == null){
			canvasPos = GameObject.Find("Phase2").transform.Find("CanvasP2")
				.GetComponent<RectTransform>();
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvasPos,
			screenPos,
			curCamera,
			out pos);
		
		return pos;
	}

	public void Focus_P1(){
		observer.FocusObject(child);
	}

	public void SetSLN(string name){
		rend.sortingLayerName = name;
	}
}
