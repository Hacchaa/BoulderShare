using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IKLookAt : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	private Bounds bounds;

	[SerializeField]
	private ThreeDWall threeDWall;
	[SerializeField]
	private Camera cam;
	[SerializeField]
	private Transform avatar = null;
	[SerializeField]
	private TwoDWall twoDWall;
	[SerializeField]
	private ThreeD threeD;
	
	// Use this for initialization
	void Start () {
		finger = FINGER_NONE;
	}

	public void ActivateLooking(){
		if (gameObject.activeSelf){
			gameObject.SetActive(false);
			threeD.SetIsLookingActivate(false);
		}else{
			Vector3 p = threeDWall.CalcWallPoint(avatar.position);
			transform.position = p;
			gameObject.SetActive(true);
			threeD.SetIsLookingActivate(true);
		}
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
			bounds = twoDWall.GetWallBounds();
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					cam.gameObject.transform.InverseTransformPoint(transform.position).z));
			
			Vector3 oldP = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					cam.gameObject.transform.InverseTransformPoint(transform.position).z));
		
			transform.Translate(p - oldP);


			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;
			Vector3 wallWorldPos = threeDWall.GetWallWorldPos();

			x = Mathf.Min(x, wallWorldPos.x + bounds.size.x/2);
			x = Mathf.Max(x, wallWorldPos.x - bounds.size.x/2);
			y = Mathf.Min(y, wallWorldPos.y + bounds.size.y/2);
			y = Mathf.Max(y, wallWorldPos.y - bounds.size.y/2);

			transform.position = threeDWall.CalcWallPoint(new Vector3(x, y, z));
			
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
