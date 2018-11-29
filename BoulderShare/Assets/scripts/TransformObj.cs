using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformObj : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	public Camera cam;
	public AvatarControl ac;
	public Transform target = null;
	public Transform avatar = null;
	[SerializeField]
	private bool isFixed = false;
	public Hold_Pose hp;
	private static string BODY_NAME = "CollisionBody";
	[SerializeField]
	private bool isRHD = false;
	[SerializeField]
	private bool isRFD = false;
	[SerializeField]
	private bool isLHD = false;
	[SerializeField]
	private bool isLFD = false;
	[SerializeField]
	private bool isBDY = false;
	[SerializeField]
	private ThreeDHolds threeDHolds;
	private float threeDHoldR;
	private Vector3 threeDHoldPos;
	[SerializeField]
	private Observer observer;
	private Bounds bounds;

	// Use this for initialization
	void Start () {
		finger = Observer.FINGER_NONE;

		if (gameObject.name.Equals("CollisionRHD")){
			isRHD = true;
		}
		if (gameObject.name.Equals("CollisionRFD")){
			isRFD = true;
		}
		if (gameObject.name.Equals("CollisionLHD")){
			isLHD = true;
		}
		if (gameObject.name.Equals("CollisionLFD")){
			isLFD = true;
		}
		if (gameObject.name.Equals("CollisionBody")){
			isBDY = true;
		}
	}	

	public Vector3 GetTargetPos(){
		return target.position;
	}

	void LateUpdate(){
		if (avatar != null){
			transform.position = avatar.position;
		}
	}

	public bool IsFixed(){
		return isFixed;
	}
	
	public void SetFixed(bool b){
		isFixed = b;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			finger = data.pointerId;
			target.position = transform.position;

			if (isRHD){
				threeDHoldR = threeDHolds.GetR((int)AvatarControl.BODYS.RH);
				threeDHoldPos = threeDHolds.GetPos((int)AvatarControl.BODYS.RH);
			}else if(isRFD){
				threeDHoldR = threeDHolds.GetR((int)AvatarControl.BODYS.RF);
				threeDHoldPos = threeDHolds.GetPos((int)AvatarControl.BODYS.RF);
			}
			else if(isLHD){
				threeDHoldR = threeDHolds.GetR((int)AvatarControl.BODYS.LH);
				threeDHoldPos = threeDHolds.GetPos((int)AvatarControl.BODYS.LH);
			}
			else if(isLFD){
				threeDHoldR = threeDHolds.GetR((int)AvatarControl.BODYS.LF);
				threeDHoldPos = threeDHolds.GetPos((int)AvatarControl.BODYS.LF);
			}else if (isBDY){
				bounds = observer.GetWallBounds();
			}
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//float len = (cam.transform.position - target.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					cam.gameObject.transform.InverseTransformPoint(target.position).z));
		
			target.position = p;

			if (isRHD || isRFD || isLHD || isLFD){
				
				//バウンド処理
				float z = target.localPosition.z;
				float boundZ = ac.CalcZPos(target.localPosition);
				
				if (z > boundZ){
					target.localPosition = 
						new Vector3(target.localPosition.x,
							target.localPosition.y,
							boundZ);
				}

				if (isFixed){
					Vector3 v = target.position - threeDHoldPos;
					/*
					Debug.Log("taget"+target.position);
					Debug.Log("3D"+threeDHoldPos);
					Debug.Log("v"+v);
					Debug.Log("v.mag"+v.magnitude);
					Debug.Log("r:"+threeDHoldR);*/
					if (v.magnitude > threeDHoldR){
						target.position = threeDHoldPos + v.normalized * threeDHoldR;
					}
				}
			}else if (isBDY){
				float x = target.localPosition.x;
				float y = target.localPosition.y;
				float z = target.localPosition.z;
				float offsetW = 0.5f;
				float zUB = ac.CalcZPos(target.localPosition) - 0.15f;
				float zLB = ac.CalcZPos(target.localPosition) - 0.5f;

				x = Mathf.Min(x, bounds.size.x/2 + offsetW);
				x = Mathf.Max(x, -(bounds.size.x/2 + offsetW));
				y = Mathf.Min(y, bounds.size.y);
				y = Mathf.Max(y, 0.0f);
				z = Mathf.Min(z, zUB);
				z = Mathf.Max(z, zLB);

				target.localPosition = new Vector3(x, y, z);
			}
			/*
			if (gameObject.name.Equals(BODY_NAME)){
				//target.position = new Vector3(p.x, p.y, ac.CalcBodyZPos(p));
				target.position = p;
			}else{
				target.position = new Vector3(p.x, p.y, ac.CalcZPos(p));
			}*/
			target.localRotation = transform.localRotation;
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			if(isBDY){
				cam.gameObject.transform.parent.position = gameObject.transform.position;
			}
			finger = Observer.FINGER_NONE;
		}
	}
}
