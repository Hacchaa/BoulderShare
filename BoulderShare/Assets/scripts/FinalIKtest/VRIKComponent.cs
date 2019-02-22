using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRIKComponent : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	protected static int finger ;
	protected const int FINGER_NONE = -10;
	[SerializeField] protected Camera cam;
	protected float baseDepth;
	protected Vector3 defaultPos;
	[SerializeField] protected Transform avatar;
	[SerializeField] protected Transform target;

	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		defaultPos = target.localPosition;
		target.position = transform.position;
		Init();
	}

	public virtual void Init(){
		ResetPos();
	}

	void Start(){

	}

	void LateUpdate(){
		OnPreLateUpdate();
		Invoke("Correct", 0.0f);
		OnPostLateUpdate();
	}

	protected virtual void OnPostLateUpdate(){

	}
	protected virtual void OnPreLateUpdate(){

	}

	public virtual void ResetPos(){
		target.localPosition = defaultPos;
	}	

	//足の位置を変えないままtargetの位置を修正する
	public virtual void ModifyPosition(){
		target.position = transform.position;
	}
	public Vector3 GetPosition(){
		return target.localPosition;
	}
	public void SetPosition(Vector3 p){
		target.localPosition = p;
	}
	public Quaternion GetRotation(){
		return target.localRotation;
	}
	public void SetRotation(Quaternion rot){
		target.localRotation = rot;
	}
	//avatarの位置に移動させる
	public virtual void Correct(){
		if (avatar != null){
			transform.position = avatar.position;
		}
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
			ModifyPosition();
		}
	}

	public virtual void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			Vector3 pOld = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					baseDepth));

			target.Translate(p - pOld, Space.World);
			OnPostDrag();
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
			ModifyPosition();
		}
	}

	protected virtual void OnPostDrag(){

	}
}
