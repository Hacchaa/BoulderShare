using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReverseTransitionArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField] private BSScreenTransition transition;
	[SerializeField] private GameObject target;
	private bool isTargetDraggable = false;
	private bool doneReverseTransition = false;
	private bool determinedProc = false;
	private int finger = MyUtility.FINGER_NONE;
	private float width;
	private float startWidth;
	private Vector2 startPos;
	private float retina;
	//screen length per second. 1画面の横ピクセル数を単位とする
	[SerializeField] private float threshold_slps = 4.0f;
	[SerializeField] private float dirLengthPT = 10.0f;
	

	public void OnPointerDown(PointerEventData data){
		isTargetDraggable = false;
		if (ExecuteEvents.CanHandleEvent<IDragHandler>(target)){
			isTargetDraggable = true;
		}

		if (isTargetDraggable && ExecuteEvents.CanHandleEvent<IInitializePotentialDragHandler>(target)){
			ExecuteEvents.Execute<IInitializePotentialDragHandler>(target, (BaseEventData) data, ExecuteEvents.initializePotentialDrag);
		}
		doneReverseTransition = false;
		determinedProc = false;
		retina = CanvasResolutionManager.GetRatioOfPtToPx();
	}

	public void OnPointerUp(PointerEventData data){

	}

	private void InitRT(float w){
		width = Screen.width;
		startWidth = w;
		ScreenTransitionManager.Instance.InitScreen(transition.GetFrom());
		transition.Ready(true);		
	}

	private void InitTarget(PointerEventData data){
		if (isTargetDraggable && ExecuteEvents.CanHandleEvent<IBeginDragHandler>(target)){
			ExecuteEvents.Execute<IBeginDragHandler>(target, (BaseEventData) data, ExecuteEvents.beginDragHandler);
		}
	}
	public void OnBeginDrag(PointerEventData data){
		if (finger == MyUtility.FINGER_NONE){
			finger = data.pointerId;
			startPos = data.position;
		}		
	} 
	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//判定
			if(!determinedProc){
				Vector2 dir = data.position - startPos;
				//Debug.Log("dir.sqrMagnitude:"+dir.sqrMagnitude);
				//Debug.Log("dirLengthPT * dirLengthPT * retina * retina:"+dirLengthPT * dirLengthPT * retina * retina);
				if (dir.sqrMagnitude > dirLengthPT * dirLengthPT * retina * retina){
					if (dir.x > 0 && (Mathf.Abs(dir.y) < dir.x)){
						doneReverseTransition = true;
						InitRT(data.position.x);
					}else{
						doneReverseTransition = false;
						InitTarget(data);
					}
					determinedProc = true;
				}
				return ;
			}

			if (doneReverseTransition){
				float diff = data.position.x - startWidth;
				float t = (width - diff) / width;
				transition.BSTransitionLerp(t);
			}else{
				if (isTargetDraggable){
					ExecuteEvents.Execute<IDragHandler>(target, (BaseEventData) data, ExecuteEvents.dragHandler);
				}				
			}
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = MyUtility.FINGER_NONE;

			if (doneReverseTransition){
				float slps = (data.delta.x / Time.deltaTime) / Screen.width;
				///Debug.Log("data.delta.x:"+data.delta.x);
				//Debug.Log("Time.deltaTime:"+Time.deltaTime);
				//Debug.Log("data.delta.x / Time.deltaTime"+ data.delta.x / Time.deltaTime);
				//Debug.Log("screen.width:"+Screen.width);
				//Debug.Log("slps:"+slps);
				if (slps > threshold_slps){
					ScreenTransitionManager.Instance.Transition(transition.GetFrom());
					return ;
				}

				float diff = data.position.x - startWidth;
				float t = (width - diff) / width;
				if (t > 0.5f){
					transition.BSTransitionWithAnim(t, false);
				}else{
					//画面遷移する
					ScreenTransitionManager.Instance.Transition(transition.GetFrom(), false);
				}
			}else{
				if (isTargetDraggable){
					ExecuteEvents.Execute<IEndDragHandler>(target, (BaseEventData) data, ExecuteEvents.endDragHandler);
				}				
			}

			isTargetDraggable = false;
			doneReverseTransition = false;
			determinedProc = false;
		}
	}
}
