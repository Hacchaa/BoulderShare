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
	[SerializeField] protected Transform faceAvatar;
	[SerializeField] protected Transform model;
	[SerializeField] private VRIKController vrIK;
	[SerializeField] private VRIKController.FullBodyMark identity;
	[SerializeField] protected MeshRenderer render;
	private Vector3 offset;
	protected Transform rootVRMark;
	[SerializeField] List<Transform> relativePosList;
	
	// Use this for initialization
	void Awake () {
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		defaultPos = target.localPosition;
		target.position = transform.position;
		if (avatar != null){
			offset = model.InverseTransformPoint(avatar.position);
		}else{
			offset = model.InverseTransformPoint(faceAvatar.position);
		}
		if (vrIK != null){
			rootVRMark = vrIK.GetRootVRMark();
		}
		Init();
	}

	public Vector3 GetOffset(){
		//return offset;
		return defaultPos;
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

	public virtual bool IsShow(){
		return render.enabled;
	}
	public virtual void Hide(){
		render.enabled = false;
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	public virtual void Show(){
		render.enabled = true;
		gameObject.layer = LayerMask.NameToLayer("AvatarControl");
	}

	public virtual void ResetPos(){
		target.localPosition = defaultPos;
	}	

	//足の位置を変えないままtargetの位置を修正する
	public virtual void ModifyPosition(){
		target.position = transform.position;
	}
	public Transform GetFaceAvatar(){
		return faceAvatar;
	}
	public Transform GetTarget(){
		return target;
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
			if (vrIK != null){
				vrIK.StoreHSState();
				vrIK.HideAll();	
				Show();			
			}
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

			Vector3 v = p - pOld;
			target.Translate(v, Space.World);

			foreach(Transform t in relativePosList){
				t.Translate(v, Space.World);
			}
			OnPostDrag();
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
			ModifyPosition();
			if(vrIK != null){
				vrIK.RepairHSState();
			}
			OnPostEndDrag();
		}

	}

	protected virtual void OnPostDrag(){

	}

	protected virtual void OnPostEndDrag(){

	}
}
