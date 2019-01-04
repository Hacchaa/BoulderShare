using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreeD : MonoBehaviour {
	[SerializeField]
	private IKControl2 ik;
	[SerializeField]
	private GameObject cameras;
	[SerializeField]
	private GameObject windowCamera;
	[SerializeField]
	private GameObject cameraObj;
	[SerializeField]
	private ThreeDWallMarks threeDWallMarks;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private Transform modelRootObj;
	[SerializeField]
	private Vector3[] centerPos;
	[SerializeField]
	private Vector3[] offsetFromBody;
	[SerializeField]
	private ThreeDWall threeDWall;

	private const float DEPTH_NORMAL_LOOKING = 5.0f;
	private const float DEPTH_NORMAL_DEFAULT = 10.0f;
	private const float DEPTH_WINDOW_DEFAULT = 2.6f;

	public bool IsIKValid(){
		return ik.IsIKActive();
	}

	public int GetWallIncline(){
		return threeDWall.GetWallIncline();
	}

	public bool IsLookingActivate(){
		return ik.IsLookingActivate();
	}

	public void SetIsLookingActivate(bool b){
		ik.SetIsLookingActivate(b);
	}

	public void SetWallIncline(int value){
		threeDWall.SetWallIncline(value);
	}

	public float GetModelSize(){
		return modelRootObj.localScale.x ;
	}

	public void SetModelSize(float value){
		modelRootObj.localScale = Vector3.one * value;
	}

	public void LookAtModel(bool isDefault = true){
		Vector3 body = ik.GetBodyPosition();
		cameraObj.transform.position = new Vector3(body.x, body.y, body.z);
		if (isDefault){
			cameras.transform.localPosition = new Vector3(0.0f, 0.0f, -DEPTH_NORMAL_LOOKING);
		}else{
			cameras.transform.localPosition = new Vector3(0.0f, 0.0f, -DEPTH_NORMAL_DEFAULT);
		}
		windowCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -DEPTH_WINDOW_DEFAULT * GetModelSize());
	}

	public void ResetCamPos(){
		Vector3 v = threeDWall.GetWallWorldPos();
		cameraObj.transform.position = new Vector3(v.x, v.y, 0.0f);
		cameraObj.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		cameras.transform.localPosition = new Vector3(0.0f, 0.0f, -DEPTH_NORMAL_DEFAULT);
		windowCamera.transform.localPosition = new Vector3(0.0f, 0.0f, -DEPTH_WINDOW_DEFAULT * GetModelSize());
	}

	public Vector3[] GetModelPosition(){
		return ik.GetPosition();
	}

	public Quaternion[] GetModelRotation(){
		return ik.GetRotation();
	}

	public void SetModelPose(Vector3[] pos, Quaternion[] rot){
		ik.SetPose(pos, rot);
	}

	public void InitModelPose(){
		ik.InitAvatar();
	}

	public void CorrectModelPose(){
		Vector3[] pos = new Vector3[Enum.GetNames(typeof(EditorManager.BODYS)).Length-1];
		Quaternion[] rot = ik.GetRotation();
		string[] onTouch = twoDWallMarks.GetTouchInfo();
		//Debug.Log(string.Join(", ", onTouch));
		int index = 0;
		Vector3 pivot = Vector3.zero;
		int n = 0;
		for(int i = (int)EditorManager.BODYS.RH ; i <= (int)EditorManager.BODYS.LF ; i++){
			if(!string.IsNullOrEmpty(onTouch[i])){
				pos[i] = threeDWallMarks.GetMarkObj(onTouch[i]).transform.position;
				pos[i] = modelRootObj.InverseTransformPoint(pos[i]);
				pivot += pos[i];
				n++;
				index += (int)Mathf.Pow(2, i);
			}
		}
		if (n != 0){
			pivot /= n;
		}
		pos[(int)EditorManager.BODYS.BODY] = pivot + centerPos[index];

		//手足位置の微調整
		for(int i = (int)EditorManager.BODYS.RH ; i <= (int)EditorManager.BODYS.LH ; i++){
			//ホールドをつかんでいない手足の位置
			if(onTouch[i] == null){
				pos[i] = offsetFromBody[i] + pos[(int)EditorManager.BODYS.BODY];
			}

			rot[i] = Quaternion.Euler(-50.0f, 0.0f, 0.0f);
		}

		for(int i = (int)EditorManager.BODYS.RF ; i <= (int)EditorManager.BODYS.LF ; i++){
			//ホールドをつかんでいない手足の位置
			if(onTouch[i] == null){
				pos[i] = offsetFromBody[i] + pos[(int)EditorManager.BODYS.BODY];
			}
		}

		//左右肘
		for(int i = (int)EditorManager.BODYS.RE ; i <= (int)EditorManager.BODYS.LE ; i++){
				pos[i] = (pos[i-4] + pos[(int)EditorManager.BODYS.BODY]) / 2;
				//rot[i]
				//pos[i].z = 0.0f;
		}
		//左右膝
		for(int i = (int)EditorManager.BODYS.RK ; i <= (int)EditorManager.BODYS.LK ; i++){
				pos[i] = (pos[i-4] + pos[(int)EditorManager.BODYS.BODY]) / 2;
				//pos[i].z = 0.0f;
		}

		ik.SetPose(pos, rot);
	}
/*
	public void CalcJointAngle(){
		Vector3[] pos = ik.GetPosition();
		Quaternion[] rot = ik.GetRotation();
		float bodyAngleY = rot[(int)EditorManager.BODYS.BODY].eulerAngles.y;
		int index = (int)EditorManager.BODYS.LF;

		Vector3 kneePos = pos[(int)EditorManager.BODYS.LK];
		Vector3 footPos = pos[index];

		//膝から足に向かうベクトルを返す
		Vector3 p = footPos - kneePos;
		float degZY = Vector2.Angle(new Vector2(0, -1), new Vecotr2())
	}*/
}
