using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ACComponent : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	public Transform target = null;
	public Transform avatar = null;
	private Bounds bounds;
	private float threeDMarkR;
	private Vector3 threeDMarkPos;
	private bool isFixed = false;
	[SerializeField]
	private int bodyType;
	[SerializeField]
	private ThreeDWallMarks threeDWallMarks;
	[SerializeField]
	private TwoDWall twoDWall;
	[SerializeField]
	private ThreeDWall threeDWall;
	[SerializeField]
	private ThreeD threeD;
	
	// Use this for initialization
	void Start () {
		finger = FINGER_NONE;
	}	

	void LateUpdate(){
		if (avatar != null){
			transform.position = avatar.position;
			target.position = avatar.position;
		}
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
			target.position = transform.position;

			if (bodyType >= (int)EditorManager.BODYS.RH && bodyType <= (int)EditorManager.BODYS.LF){
				GameObject tmp = threeDWallMarks.GetMarkObj(bodyType);
				if (tmp != null){
					threeDMarkR = tmp.transform.localScale.x / ThreeDWallMarks.THREED_MARK_SIZE / 2;
					threeDMarkPos = tmp.transform.position;
					isFixed = true;
				}else{
					isFixed = false;
				}
			}

			if (bodyType == (int)EditorManager.BODYS.BODY){
				bounds = twoDWall.GetWallBounds();
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

			if (bodyType >= (int)EditorManager.BODYS.RH && bodyType <= (int)EditorManager.BODYS.LF){
				
				//バウンド処理
				Vector3 vec = threeDWall.CalcWallPoint(target.position);
				//float z = target.localPosition.z;
				//float boundZ = threeDWall.CalcZPos(target.localPosition);
				
				if (target.position.z > vec.z){
					target.position = vec;
				}
				
				if (isFixed){
					Vector3 v = target.position - threeDMarkPos;
					/*
					Debug.Log("taget"+target.position);
					Debug.Log("3D"+threeDHoldPos);
					Debug.Log("v"+v);
					Debug.Log("v.mag"+v.magnitude);
					Debug.Log("r:"+threeDHoldR);*/
					Debug.Log(v.magnitude +" > " + threeDMarkR);
					if (v.magnitude > threeDMarkR){
						target.position = threeDMarkPos + v.normalized * threeDMarkR;
					}
				}
			}else if (bodyType == (int)EditorManager.BODYS.BODY){
				
				float x = target.position.x;
				float y = target.position.y;
				float z = target.position.z;
				float offsetW = 0.5f;
				Vector3 wallWorldPos = threeDWall.GetWallWorldPos();

				x = Mathf.Min(x, wallWorldPos.x + bounds.size.x/2 + offsetW);
				x = Mathf.Max(x, wallWorldPos.x - (bounds.size.x/2 + offsetW));
				y = Mathf.Min(y, wallWorldPos.y + bounds.size.y/2);
				y = Mathf.Max(y, wallWorldPos.y - bounds.size.y/2);

				target.position = new Vector3(x, y, z);

				Vector3 vec = threeDWall.CalcWallPoint(new Vector3(x, y, z));
				float zUB = vec.z - 0.15f;
				float zLB = vec.z - 0.5f;

				if (target.position.z > zUB){
					target.position = new Vector3(x, y, zUB);
				}else if(target.position.z < zLB){
					target.position = new Vector3(x, y, zLB);
				}else{
					target.position = new Vector3(x, y, z);
				}

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
			if (bodyType == (int)EditorManager.BODYS.BODY){
				threeD.LookAtModel();
			}
			/*
			if(isBDY){
				cam.gameObject.transform.parent.position = gameObject.transform.position;
			}*/
			finger = Observer.FINGER_NONE;
		}
	}
}
