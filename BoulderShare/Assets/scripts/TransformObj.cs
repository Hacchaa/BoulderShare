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
	private bool isFixed = false;
	public Hold_Pose hp;
	private static string BODY_NAME = "CollisionBody";
	private bool isRHD = false;
	private bool isRFD = false;
	private bool isLHD = false;
	private bool isLFD = false;
	private bool isBDY = false;
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
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//float len = (cam.transform.position - target.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					target.position.z - cam.transform.position.z));
		
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
					float r = 0.0f;
					Vector3 pos = Vector3.zero;
					if (isRHD){
						r = hp.GetR((int)AvatarControl.BODYS.RH);
						pos = hp.GetHoldPos((int)AvatarControl.BODYS.RH);
					}else if(isRFD){
						r = hp.GetR((int)AvatarControl.BODYS.RF);
						pos = hp.GetHoldPos((int)AvatarControl.BODYS.RF);
					}
					else if(isLHD){
						r = hp.GetR((int)AvatarControl.BODYS.LH);
						pos = hp.GetHoldPos((int)AvatarControl.BODYS.LH);
					}
					else if(isLFD){
						r = hp.GetR((int)AvatarControl.BODYS.LF);
						pos = hp.GetHoldPos((int)AvatarControl.BODYS.LF);
					}

					Vector3 v = target.localPosition - pos;

					if (v.magnitude > r){
						target.localPosition = pos + v.normalized * r;
					}
				}
			}else if (isBDY){
				Vector2 o = new Vector2(0.0f, 0.6f);
				float bodyR = 0.5f;
				float z =  target.localPosition.z;
				Vector2 v = (Vector2)target.localPosition - o;
				Vector2 tmp = target.localPosition;
				if (v.magnitude > bodyR){
					tmp = o + v.normalized * bodyR;
				}
				float zUB = ac.CalcZPos(tmp) - 0.15f;
				float zLB = -0.35f;

				z = Mathf.Min(z, zUB);
				z = Mathf.Max(z, zLB);

				target.localPosition = new Vector3(tmp.x, tmp.y, z);
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
			finger = Observer.FINGER_NONE;
		}
	}
}
