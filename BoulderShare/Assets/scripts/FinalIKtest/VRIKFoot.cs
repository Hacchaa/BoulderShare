using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class VRIKFoot : VRIKComponent
{
	[SerializeField] private Transform footAvatar;
	private Quaternion baseRotFoot;
	private Quaternion baseRotToe;
	private Vector3 baseVec;
	[SerializeField] Vector3 initRotEuler;
	[SerializeField] Vector3 initToeRotEuler;
	[SerializeField] Vector3 baseTargetRotEuler;
	[SerializeField] Transform thighAvatar;
	[SerializeField] Transform bendTarget;
	[SerializeField] private float r = 0.5f;
	[SerializeField] private Transform centerAvatar;

	public override void Init(){
		ResetPos();
		//baseTargetRot = target.rotation;
		baseRotFoot = Quaternion.Euler(initRotEuler);
		baseRotToe = Quaternion.Euler(initToeRotEuler);
		//baseVec = avatar.position - thighAvatar.position;
		//baseVec = transform.position - bendTarget.position;
	}

	void Update(){
		//target.rotation = Quaternion.FromToRotation(baseVec, (transform.position - bendTarget.position)) * baseTargetRot;
		if (centerAvatar != null){
			Vector3 dir = rootVRMark.InverseTransformPoint(target.position) - rootVRMark.InverseTransformPoint(centerAvatar.position);
			if (dir.magnitude > r){
				target.localPosition = rootVRMark.InverseTransformPoint(centerAvatar.position) + dir.normalized * r;
			}
		}
	}
	//modifyPosition()を無効にする
	public override void ModifyPosition(){
	}
	public override void Correct(){
		if (avatar != null){
			transform.position = footAvatar.position;
			//target.position = transform.position;
		}
		//Quaternion rot = footAvatar.rotation;
		//Debug.Log("baseRotFoot:"+baseRotFoot.eulerAngles);
		footAvatar.localRotation = baseRotFoot;
		avatar.localRotation = baseRotToe;
		//target.rotation = footAvatar.rotation * Quaternion.Euler(baseTargetRotEuler);
		//footAvatar.rotation = rot;
	}
}
