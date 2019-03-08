using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanModel : MonoBehaviour {
	[SerializeField] private VRIKController fIK;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private ThreeDWallMarks threeDWallMarks;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private Transform modelRootObj;
	[SerializeField] private ThreeDWall threeDWall;
	[SerializeField] private Vector3[] centerPos;
	[SerializeField] private Vector3[] offsetFromBody;
	[SerializeField] private Vector3[] offsetTouching;
	[SerializeField] private Vector3 offsetFromCenterToHead;
	private const float DEPTH3D_LOOKING = -5.0f;


	public float GetModelSize(){
		return modelRootObj.localScale.x ;
	}

	public void SetModelSize(float value){
		modelRootObj.localScale = Vector3.one * value;
	}

	public void LookAtModel(){
		Vector3 body = fIK.GetBodyPosition();
		cameraManager.Reset3DCamPosAndDepth();
		cameraManager.SetRootWorldPos(body);
		cameraManager.Set3DDepth(DEPTH3D_LOOKING);
	}

	public Vector3[] GetModelPosition(){
		return fIK.GetPositions();
	}

	public Quaternion[] GetModelRotation(){
		return fIK.GetRotations();
	}

	public Vector3 GetModelBodyPosition(){
		return fIK.GetBodyPosition();
	}

	public void CopyModelPose(Transform copyTo, Vector3[] positions, Quaternion[] rotations){
		Vector3[] posOrigin = GetModelPosition();
		Quaternion[] rotOrigin = GetModelRotation();

		SetModelPose(positions, rotations);

		fIK.DoVRIK();

		CopyRecursively(fIK.GetObj(VRIKController.FullBodyMark.Body).transform , copyTo);

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
		fIK.SetPositions(pos);
		fIK.SetRotations(rots);
	}

	public void InitModelPose(){
		//ik.InitAvatar();
		fIK.InitAvatar();
	}

	//VRIKController.FullBodyMarkからTwoDMark.HFTypeに変換
	public int Convert(int type){
		switch(type){
			case (int)VRIKController.FullBodyMark.LeftHand : return (int)TwoDMark.HFType.LH ;
			case (int)VRIKController.FullBodyMark.RightHand : return (int)TwoDMark.HFType.RH ;
			case (int)VRIKController.FullBodyMark.LeftFoot : return (int)TwoDMark.HFType.LF ;
			case (int)VRIKController.FullBodyMark.RightFoot : return (int)TwoDMark.HFType.RF ;
		}
		return -1;
	}


	public void CorrectModelPose(){
		
		Vector3[] pos = new Vector3[Enum.GetNames(typeof(VRIKController.FullBodyMark)).Length];
		Quaternion[] rot = fIK.GetRotations();
		string[] onTouch = twoDWallMarks.GetTouchInfo();
		Debug.Log(string.Join(", ", onTouch));
		int index = 0;
		Vector3 pivot = Vector3.zero;
		int n = 0;
		int type = -1;
		for(int i = (int)VRIKController.FullBodyMark.LeftHand ; i <= (int)VRIKController.FullBodyMark.RightFoot ; i++){
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

		pos[(int)VRIKController.FullBodyMark.Head] = pivot + threeDWall.CalcWorldSubVec(centerPos[index] + offsetFromCenterToHead);
		pos[(int)VRIKController.FullBodyMark.Look] = pos[(int)VRIKController.FullBodyMark.Head] 
			+ threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)VRIKController.FullBodyMark.Body] = pos[(int)VRIKController.FullBodyMark.Head] 
			- threeDWall.CalcWorldSubVec(fIK.GetOffsetFromRoot(VRIKController.FullBodyMark.Head));
		pos[(int)VRIKController.FullBodyMark.Chest] = pos[(int)VRIKController.FullBodyMark.Body] 
			+ threeDWall.CalcWorldSubVec(fIK.GetOffsetFromRoot(VRIKController.FullBodyMark.Chest) + Vector3.forward * 0.5f);
		pos[(int)VRIKController.FullBodyMark.Pelvis] = pos[(int)VRIKController.FullBodyMark.Body] 
			+ threeDWall.CalcWorldSubVec(fIK.GetOffsetFromRoot(VRIKController.FullBodyMark.Pelvis) - Vector3.up * 0.1f);
		
		//手足位置の微調整
		for(int i = (int)VRIKController.FullBodyMark.LeftHand ; i <= (int)VRIKController.FullBodyMark.RightFoot ; i++){
			//ホールドをつかんでいない手足の位置
			type = Convert(i);
			if(onTouch[type] == null){
				pos[i] = pos[(int)VRIKController.FullBodyMark.Body] 
					+ threeDWall.CalcWorldSubVec(fIK.GetOffsetFromRoot((VRIKController.FullBodyMark)i) + offsetFromBody[type]);
			}
		}

		//左右肘膝
		pos[(int)VRIKController.FullBodyMark.LeftElbow] = (pos[(int)VRIKController.FullBodyMark.Chest] + pos[(int)VRIKController.FullBodyMark.LeftHand]) / 2f 
			- threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)VRIKController.FullBodyMark.RightElbow] = (pos[(int)VRIKController.FullBodyMark.Chest] + pos[(int)VRIKController.FullBodyMark.RightHand]) / 2f 
			- threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)VRIKController.FullBodyMark.LeftKnee] = (pos[(int)VRIKController.FullBodyMark.Pelvis] + pos[(int)VRIKController.FullBodyMark.LeftFoot]) / 2f 
			+ threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		pos[(int)VRIKController.FullBodyMark.RightKnee] = (pos[(int)VRIKController.FullBodyMark.Pelvis] + pos[(int)VRIKController.FullBodyMark.RightFoot]) / 2f 
			+ threeDWall.CalcWorldSubVec(Vector3.forward * 0.5f);
		
		/*for(int i = (int)VRIKController.FullBodyMark.LeftElbow ; i <= (int)VRIKController.FullBodyMark.RightElbow ; i++){
				pos[i] = (pos[(int)VRIKController.FullBodyMark.Pelvis] + pos[(int)VRIKController.FullBodyMark.LeftHand]) / 2f + Vector3.forward * 0.5f;
				//rot[i]
				//pos[i].z = 0.0f;
		}
		//左右膝
		for(int i = (int)VRIKController.FullBodyMark.LeftKnee ; i <= (int)VRIKController.FullBodyMark.RightKnee ; i++){
				pos[i] = (pos[i-4] + pos[(int)EditorManager.BODYS.BODY]) / 2;
				//pos[i].z = 0.0f;
		}*/
		SetModelPose(pos, rot);
	}

}
