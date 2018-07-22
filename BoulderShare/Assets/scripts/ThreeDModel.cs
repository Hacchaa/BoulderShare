using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDModel : MonoBehaviour {
	public GameObject[] switchObjs;
	public Transform parent;
	public Transform modelsize;
	public Transform model;
	public Transform modelSizeParent;
	private IKControl ik;
	public enum Mode{DEFAULT, MODEL_SIZE, WINDOW, POSE};
	private Vector3[] tmpV;
	private Quaternion[] tmpR;
	public Hold_Pose hp;
	public AvatarControl ac;
	private bool isReturned;
	public Shield shield;
	// Use this for initialization
	void Start () {
		isReturned = false;
		ik = model.gameObject.GetComponent<IKControl>();
	}
	
	public void ChangeMode(int mode){
	
		if(mode == (int)ThreeDModel.Mode.MODEL_SIZE){
			LendModel();
			isReturned = false;
		}else{
			if(!isReturned){
				ReturnModel();
				isReturned = true;
			}
		}

		if(mode == (int)ThreeDModel.Mode.WINDOW){
			switchObjs[(int)ThreeDModel.Mode.WINDOW].SetActive(true);
		}else{
			switchObjs[(int)ThreeDModel.Mode.WINDOW].SetActive(false);
		} 

		if(mode == (int)ThreeDModel.Mode.POSE){
			switchObjs[(int)ThreeDModel.Mode.POSE].SetActive(true);
		}else{
			switchObjs[(int)ThreeDModel.Mode.POSE].SetActive(false);
		}
	}

	public void OpenPose(){
		ChangeMode((int)ThreeDModel.Mode.POSE);
		tmpV = ik.GetPosition();
		tmpR = ik.GetRotation();

		PoseProc();
		shield.OpenIgnoreTouch();
	}

	private void PoseProc(){
		hp.Sync();
		Vector3[] v = ik.GetPosition();
		Quaternion[] r = ik.GetRotation();
		Vector3[] holdPos = hp.GetHoldsPos();

		if (holdPos[(int)AvatarControl.BODYS.RH] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.RH)){
				v[(int)AvatarControl.BODYS.RH] = holdPos[(int)AvatarControl.BODYS.RH];
				ac.SetFixed((int)AvatarControl.BODYS.RH, true);
			}			
		}

		if (holdPos[(int)AvatarControl.BODYS.RF] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.RF)){
				v[(int)AvatarControl.BODYS.RF] = holdPos[(int)AvatarControl.BODYS.RF];
				ac.SetFixed((int)AvatarControl.BODYS.RF, true);
			}			
		}
		if (holdPos[(int)AvatarControl.BODYS.LH] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.LH)){
				v[(int)AvatarControl.BODYS.LH] = holdPos[(int)AvatarControl.BODYS.LH];
				ac.SetFixed((int)AvatarControl.BODYS.LH, true);
			}			
		}
		if (holdPos[(int)AvatarControl.BODYS.LF] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.LF)){
				v[(int)AvatarControl.BODYS.LF] = holdPos[(int)AvatarControl.BODYS.LF];
				ac.SetFixed((int)AvatarControl.BODYS.LF, true);
			}			
		}
			
		ik.SetPose(v, r);
	}

	public void Cancel(){
		ChangeMode((int)ThreeDModel.Mode.WINDOW);
		ik.SetPose(tmpV, tmpR);
		shield.CloseIgnoreTouch();
	}

	public void Apply(){
		ChangeMode((int)ThreeDModel.Mode.WINDOW);
		shield.CloseIgnoreTouch();
	}
	

	private void LendModel(){
		tmpV = ik.GetPosition();
		tmpR = ik.GetRotation();

		ik.InitAvatar();
		Vector3 local = modelsize.localPosition;
		modelsize.parent = modelSizeParent;
		modelsize.localPosition = local;
		modelsize.localScale = Vector3.one;
	}

	private void ReturnModel(){
		Vector3 local = modelsize.localPosition;
		modelsize.parent = parent;
		modelsize.localPosition = local;
		modelsize.localScale = Vector3.one;
		if(tmpV != null){
			ik.SetPose(tmpV, tmpR);
			tmpV = null;
		}
	}

	public IKControl GetIK(){
		return ik;
	}
}
