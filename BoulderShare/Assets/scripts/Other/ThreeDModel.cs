using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
	[SerializeField]
	private ThreeDHolds threeDHolds;
	private static float CAM_BASE_DEPTH = 3.4f;
	[SerializeField]
	private Camera windowCam;
	[SerializeField]
	private Camera poseCam;
	[SerializeField]
	private HScenes hScenes;
	private bool isCamResetInLateUpdate = false;

	// Use this for initialization
	void Start () {
		isReturned = true;
		ik = model.gameObject.GetComponent<IKControl>();
	}

	//OnAnimatorIK()の後に呼び出す
	void LateUpdate(){
		if(isCamResetInLateUpdate){
			SetCamPos(windowCam);
			isCamResetInLateUpdate = false;
		}
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
			isCamResetInLateUpdate = true;
		}else{
			switchObjs[(int)ThreeDModel.Mode.WINDOW].SetActive(false);
		} 

		if(mode == (int)ThreeDModel.Mode.POSE){
			switchObjs[(int)ThreeDModel.Mode.POSE].SetActive(true);
			SetCamPos(poseCam);
		}else{
			switchObjs[(int)ThreeDModel.Mode.POSE].SetActive(false);
		}
	}

	public void SetWinCamPosition(){
		isCamResetInLateUpdate = true;
	}

	private void SetCamPos(Camera cam){
		float z;
		Vector3 p = ac.GetPos((int)AvatarControl.BODYS.BODY);
		Debug.Log("p"+p);
		cam.gameObject.transform.parent.position = p;
		cam.gameObject.transform.parent.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		z = modelsize.localScale.x * CAM_BASE_DEPTH;
		cam.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, -z);

	}

	public void OpenPose(){
		tmpV = ik.GetPosition();
		tmpR = ik.GetRotation();

		PoseProc2();
		shield.OpenIgnoreTouch();
		ChangeMode((int)ThreeDModel.Mode.POSE);
	}

	private void PoseProc2(){
		//hp.Sync();
		Vector3[] v = ik.GetPosition();
		Quaternion[] r = ik.GetRotation();
		Vector3[] holdPos = threeDHolds.CalcBodyPositions();
		string[] curHoldsInPose = hScenes.GetCurHoldsInPose();
		string[] curHolds = hScenes.GetCurHolds2();
		bool isPosable = hScenes.IsCurScenePosable();

		//ホールドをつかんでいるかどうか調べる
		bool noTouch = true;
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if (!String.IsNullOrEmpty(curHolds[i])){
				noTouch = false;
				ac.SetFixed(i, true);
			}
		}

		//ポーズが保存されているかどうか
		if(!isPosable){
			Debug.Log("!isPosable");
			v = holdPos;
		}else{
			int c = 0;
			for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
				//現在つかんでいるホールドとポーズが記録しているホールドが違う場合
				if(!String.IsNullOrEmpty(curHolds[i]) && !curHolds[i].Equals(curHoldsInPose[i])){
					Debug.Log("change "+i);
					v[i] = holdPos[i];
					c++;
				}
			}
			//つかんでいたホールドがすべて変わった場合
			if (c == 4 && !noTouch){
				Debug.Log("all changed");
				v = holdPos;
			}
		}
		/*
		if (holdPos[(int)AvatarControl.BODYS.RH] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.RH)){
				v[(int)AvatarControl.BODYS.RH] = holdPos[(int)AvatarControl.BODYS.RH];
				Debug.Log("apply RH");
				ac.SetFixed((int)AvatarControl.BODYS.RH, true);
			}			
		}else{
			v[(int)AvatarControl.BODYS.RH] += offset;
		}

		if (holdPos[(int)AvatarControl.BODYS.RF] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.RF)){
				v[(int)AvatarControl.BODYS.RF] = holdPos[(int)AvatarControl.BODYS.RF];
				Debug.Log("apply RF");
				ac.SetFixed((int)AvatarControl.BODYS.RF, true);
			}			
		}else{
			v[(int)AvatarControl.BODYS.RF] += offset;
		}

		if (holdPos[(int)AvatarControl.BODYS.LH] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.LH)){
				v[(int)AvatarControl.BODYS.LH] = holdPos[(int)AvatarControl.BODYS.LH];
				Debug.Log("apply LH");
				ac.SetFixed((int)AvatarControl.BODYS.LH, true);
			}			
		}else{
			v[(int)AvatarControl.BODYS.LH] += offset;
		}

		if (holdPos[(int)AvatarControl.BODYS.LF] != Vector3.zero){
			if (!ac.IsFixed((int)AvatarControl.BODYS.LF)){
				v[(int)AvatarControl.BODYS.LF] = holdPos[(int)AvatarControl.BODYS.LF];
				Debug.Log("apply LF");
				ac.SetFixed((int)AvatarControl.BODYS.LF, true);
			}			
		}else{
			v[(int)AvatarControl.BODYS.LF] += offset;
		}

		if (!ac.IsFixed((int)AvatarControl.BODYS.BODY)){
			v[(int)AvatarControl.BODYS.BODY] = holdPos[(int)AvatarControl.BODYS.BODY];
			Debug.Log("apply BODY");
			ac.SetFixed((int)AvatarControl.BODYS.BODY, true);
		}	*/
			
		ik.SetPose(v, r);
	}
/*
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
	}*/

	public void Cancel(){
		ik.SetPose(tmpV, tmpR);
		shield.CloseIgnoreTouch();
		ChangeMode((int)ThreeDModel.Mode.WINDOW);
	}

	public void Apply(){
		shield.CloseIgnoreTouch();
		ChangeMode((int)ThreeDModel.Mode.WINDOW);
	}
	

	private void LendModel(){
		tmpV = ik.GetPosition();
		tmpR = ik.GetRotation();

		ik.InitAvatar();
		Vector3 local = modelsize.localPosition;
		modelsize.parent = modelSizeParent;
		modelsize.localPosition = local;
		modelsize.localRotation = Quaternion.Euler(0, 0, 0);
	}

	private void ReturnModel(){
		Vector3 local = modelsize.localPosition;
		modelsize.parent = parent;
		modelsize.localPosition = local;
		modelsize.localRotation = Quaternion.Euler(0, 0, 0);
		if(tmpV != null){
			ik.SetPose(tmpV, tmpR);
			tmpV = null;
		}
	}

	public IKControl GetIK(){
		return ik;
	}
}
