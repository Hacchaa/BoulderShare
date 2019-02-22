using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
public class VRIKFoot : VRIKComponent
{
	[SerializeField] private Transform footAvatar;
	private Quaternion baseRot;
	private Quaternion baseTargetRot;
	[SerializeField] Vector3 initRotEuler;
	[SerializeField] Vector3 baseTargetRotEuler;

	public override void Init(){
		ResetPos();
		baseTargetRot = target.rotation;
		baseRot = Quaternion.Euler(initRotEuler);
	}

	public override void Correct(){
		if (avatar != null){
			transform.position = avatar.position;
		}
		Quaternion rot = footAvatar.rotation;
		//Debug.Log("baseRot:"+baseRot.eulerAngles);
		footAvatar.localRotation = baseRot;
		target.rotation = footAvatar.rotation * Quaternion.Euler(baseTargetRotEuler);
		footAvatar.rotation = rot;
	}
}
