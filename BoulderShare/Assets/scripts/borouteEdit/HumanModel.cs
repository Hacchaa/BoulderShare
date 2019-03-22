using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanModel : MonoBehaviour {
	[SerializeField] private FBBIKController fIK;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private ThreeDWallMarks threeDWallMarks;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private Transform modelRootObj;
	[SerializeField] private ThreeDWall threeDWall;
	[SerializeField] private Vector3[] centerPos;
	[SerializeField] private Vector3[] offsetFromBody;
	[SerializeField] private Vector3[] offsetTouching;
	[SerializeField] private Vector3 offsetFromCenterToHead;
	[SerializeField] private GameObject marks;

	public void HideMarks(){
		marks.SetActive(false);
	}

	public void ShowMarks(){
		marks.SetActive(true);
	}

	public float GetModelSize(){
		return modelRootObj.localScale.x ;
	}

	public void SetModelSize(float value){
		modelRootObj.localScale = Vector3.one * value;
	}

	public void LookAtModel(float depth = CameraManager.CAMERA3D_DEPTH_LOOKING){
		Vector3 body = fIK.GetWorldPosition(MyUtility.FullBodyMark.Body);
		Debug.Log("lookat body:"+body);
		cameraManager.Reset3DCamPosAndDepth();
		cameraManager.SetRootWorldPos(body);
		cameraManager.Set3DDepth(depth);
	}

	public void SetCamAxisAsModelPos(){
		Vector3 body = fIK.GetWorldPosition(MyUtility.FullBodyMark.Body);
		cameraManager.SetRootPosWithFixedHierarchyPos(body);
	}

	public Vector3[] GetModelPosition(){
		return fIK.GetPositions();
	}

	public Quaternion[] GetModelRotation(){
		return fIK.GetRotations();
	}

	public Vector3 GetModelBodyPosition(){
		return fIK.GetWorldPosition(MyUtility.FullBodyMark.Body);
	}

	public void CopyModelPose(Transform copyTo, Vector3[] positions, Quaternion[] rotations){
		Vector3[] posOrigin = GetModelPosition();
		Quaternion[] rotOrigin = GetModelRotation();

		SetModelPose(positions, rotations);

		fIK.DoVRIK();

		CopyRecursively(fIK.GetModelRoot() , copyTo);

		SetModelPose(posOrigin, rotOrigin);
	}

	private void CopyRecursively(Transform from, Transform to){
		Transform[] fromAllChildren = from.GetComponentsInChildren<Transform>(true);
		Transform[] toAllChildren = to.GetComponentsInChildren<Transform>(true);
		to.position = from.position;
		to.rotation = from.rotation;

		for(int i = 0 ; i < fromAllChildren.Length ; i++){
			toAllChildren[i].position = fromAllChildren[i].position;
			toAllChildren[i].rotation = fromAllChildren[i].rotation;
		}
	}

	public void SetModelPose(Vector3[] pos, Quaternion[] rots){
		fIK.SetPose(pos, rots);
	}

	public void InitModelPose(){
		//ik.InitAvatar();
		fIK.InitMarks();
	}

	//MyUtility.FullBodyMarkからTwoDMark.HFTypeに変換
	public int Convert(int type){
		switch(type){
			case (int)MyUtility.FullBodyMark.LeftHand : return (int)TwoDMark.HFType.LH ;
			case (int)MyUtility.FullBodyMark.RightHand : return (int)TwoDMark.HFType.RH ;
			case (int)MyUtility.FullBodyMark.LeftFoot : return (int)TwoDMark.HFType.LF ;
			case (int)MyUtility.FullBodyMark.RightFoot : return (int)TwoDMark.HFType.RF ;
		}
		return -1;
	}


	public void CorrectModelPose(){
		
		Vector3[] pos = new Vector3[Enum.GetNames(typeof(MyUtility.FullBodyMark)).Length];
		Quaternion[] rot = fIK.GetRotations();
		string[] onTouch = twoDWallMarks.GetTouchInfo();
		Debug.Log(string.Join(", ", onTouch));
		int index = 0;
		Vector3 pivot = Vector3.zero;
		int n = 0;
		int type = -1;
		for(int i = (int)MyUtility.FullBodyMark.LeftHand ; i <= (int)MyUtility.FullBodyMark.RightFoot ; i++){
			type = Convert(i);
			Debug.Log("i="+i+", type="+type);
			if(!string.IsNullOrEmpty(onTouch[type])){
				pos[i] = threeDWallMarks.GetMarkObj(onTouch[type]).transform.position;
				Debug.Log(":"+i+pos[i]);
				pos[i] = modelRootObj.InverseTransformPoint(pos[i]);
				Debug.Log(":"+i+pos[i]);
				pos[i] += threeDWall.CalcWorldSubVec(offsetTouching[type]);
				pivot += pos[i];
				n++;
				index += (int)Mathf.Pow(2, type);
			}
		}
		if (n != 0){
			pivot /= n;
		}
		Debug.Log("index:"+index);

		if (index == 0){
			InitModelPose();
			return ;
		}

		Debug.Log("pivot:"+pivot);
	
		pos[(int)MyUtility.FullBodyMark.Body] = pivot + threeDWall.CalcWorldSubVec(centerPos[index]);
		Debug.Log("body:"+pos[(int)MyUtility.FullBodyMark.Body]);

		Vector3 modelCenter = pos[(int)MyUtility.FullBodyMark.Body] - threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.Body));
		Debug.Log("modelCenter:"+modelCenter);

		pos[(int)MyUtility.FullBodyMark.Look] = modelCenter + threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.Look));
		Debug.Log("look:"+pos[(int)MyUtility.FullBodyMark.Look]);

		pos[(int)MyUtility.FullBodyMark.LeftShoulder] = modelCenter + threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.LeftShoulder));
		Debug.Log("ls:"+pos[(int)MyUtility.FullBodyMark.LeftShoulder]);

		pos[(int)MyUtility.FullBodyMark.RightShoulder] = modelCenter + threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.RightShoulder));
		Debug.Log("rs:"+pos[(int)MyUtility.FullBodyMark.RightShoulder]);

		pos[(int)MyUtility.FullBodyMark.LeftPelvis] = modelCenter + threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.LeftPelvis));
		Debug.Log("lp:"+pos[(int)MyUtility.FullBodyMark.LeftPelvis]);

		pos[(int)MyUtility.FullBodyMark.RightPelvis] = modelCenter + threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.RightPelvis));
		Debug.Log("rp:"+pos[(int)MyUtility.FullBodyMark.RightPelvis]);
/*
		pos[(int)MyUtility.FullBodyMark.Head] = pivot + threeDWall.CalcWorldSubVec(centerPos[index] + offsetFromCenterToHead);
		pos[(int)MyUtility.FullBodyMark.Look] = pos[(int)MyUtility.FullBodyMark.Head] 
			+ threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)MyUtility.FullBodyMark.Body] = pos[(int)MyUtility.FullBodyMark.Head] 
			- threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.Head));
		pos[(int)MyUtility.FullBodyMark.Chest] = pos[(int)MyUtility.FullBodyMark.Body] 
			+ threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.Chest) + Vector3.forward * 0.5f);
		pos[(int)MyUtility.FullBodyMark.Pelvis] = pos[(int)MyUtility.FullBodyMark.Body] 
			+ threeDWall.CalcWorldSubVec(fIK.GetInitPosition(MyUtility.FullBodyMark.Pelvis) - Vector3.up * 0.1f);*/
		
		//手足位置の微調整
		for(int i = (int)MyUtility.FullBodyMark.LeftHand ; i <= (int)MyUtility.FullBodyMark.RightFoot ; i++){
			//ホールドをつかんでいない手足の位置
			type = Convert(i);
			if(onTouch[type] == null){
				pos[i] = modelCenter + threeDWall.CalcWorldSubVec(fIK.GetInitPosition((MyUtility.FullBodyMark)i));
			}
		}

		//左右肘膝
		
		pos[(int)MyUtility.FullBodyMark.LeftElbow] = (pos[(int)MyUtility.FullBodyMark.LeftShoulder] + pos[(int)MyUtility.FullBodyMark.LeftHand]) / 2f 
			- threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)MyUtility.FullBodyMark.RightElbow] = (pos[(int)MyUtility.FullBodyMark.RightShoulder] + pos[(int)MyUtility.FullBodyMark.RightHand]) / 2f 
			- threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)MyUtility.FullBodyMark.LeftKnee] = (pos[(int)MyUtility.FullBodyMark.LeftPelvis] + pos[(int)MyUtility.FullBodyMark.LeftFoot]) / 2f 
			+ threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)MyUtility.FullBodyMark.RightKnee] = (pos[(int)MyUtility.FullBodyMark.RightPelvis] + pos[(int)MyUtility.FullBodyMark.RightFoot]) / 2f 
			+ threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		
		/*for(int i = (int)MyUtility.FullBodyMark.LeftElbow ; i <= (int)MyUtility.FullBodyMark.RightElbow ; i++){
				pos[i] = (pos[(int)MyUtility.FullBodyMark.Pelvis] + pos[(int)MyUtility.FullBodyMark.LeftHand]) / 2f + Vector3.forward * 0.5f;
				//rot[i]
				//pos[i].z = 0.0f;
		}
		//左右膝
		for(int i = (int)MyUtility.FullBodyMark.LeftKnee ; i <= (int)MyUtility.FullBodyMark.RightKnee ; i++){
				pos[i] = (pos[i-4] + pos[(int)EditorManager.BODYS.BODY]) / 2;
				//pos[i].z = 0.0f;
		}*/
		SetModelPose(pos, rot);
	}

}
