using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class VRIKFoot : VRIKComponent
{
	[SerializeField] private Transform footAvatar;
	private Quaternion baseRot;
	private Quaternion baseTargetRot;
	private Vector3 baseVec;
	[SerializeField] Vector3 initRotEuler;
	[SerializeField] Vector3 baseTargetRotEuler;
	[SerializeField] Transform thighAvatar;
	[SerializeField] Transform bendTarget;
	public override void Init(){
		ResetPos();
		baseTargetRot = target.rotation;
		//baseRot = Quaternion.Euler(initRotEuler);
		//baseVec = avatar.position - thighAvatar.position;
		baseVec = transform.position - bendTarget.position;
	}

	void Update(){
		target.rotation = Quaternion.FromToRotation(baseVec, (transform.position - bendTarget.position)) * baseTargetRot;
	}

	public override void Correct(){
		if (avatar != null){
			transform.position = avatar.position;
			//target.position = transform.position;
		}
		Quaternion rot = footAvatar.rotation;
		//Debug.Log("baseRot:"+baseRot.eulerAngles);
		footAvatar.localRotation = baseRot;
		target.rotation = footAvatar.rotation * Quaternion.Euler(baseTargetRotEuler);
		footAvatar.rotation = rot;
	}
}
