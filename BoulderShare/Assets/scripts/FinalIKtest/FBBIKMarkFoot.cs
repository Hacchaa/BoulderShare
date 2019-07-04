using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkFoot : FBBIKMarkHF
{
	private Quaternion storedRot;
	private Quaternion storedLocalRot;
	private Quaternion fixRot;
	private Vector3 baseDir;
	private float baseLength;
	[SerializeField] private Transform footAvatar;
	[SerializeField] private Transform thighAvatar;
	[SerializeField] private Vector3 initAngles;
	[SerializeField] private float maxDeg = 90.0f;
	[SerializeField] private float bendAngle = 0.0f;
	public override void Init(){
		//Debug.Log("init");
		base.Init();
		//storedRot = footAvatar.rotation;
		storedLocalRot = footAvatar.localRotation;

		baseDir = avatar.position - thighAvatar.position;
		baseLength = baseDir.magnitude;
		//baseDir = avatar.position - footAvatar.position;
	}

	public void AddBendAngle(float f){
		bendAngle += f;
	}

	void Update(){
		if (isInit){
			FixRot();
		}
	}
	public void FixRot(){/*
		Debug.Log("storedRot:"+storedRot.eulerAngles);
		Debug.Log("storedLocalRot:"+storedLocalRot.eulerAngles);
		//target.rotation
		Quaternion tmp = footAvatar.rotation;
		footAvatar.localRotation = storedLocalRot;
		Debug.Log("footAvatarLocal:"+footAvatar.localRotation.eulerAngles);
		Debug.Log("footAvatar:"+footAvatar.rotation.eulerAngles);
		target.localRotation = Quaternion.Euler(initAngles) * Quaternion.RotateTowards(footAvatar.rotation, storedRot,  -1.0f);
		footAvatar.rotation = tmp;

		Debug.Log("target.localRotation:"+target.localRotation.eulerAngles);*/

		Vector3 curDir = avatar.position - thighAvatar.position;
		//footAvatar.localRotation = storedLocalRot;
		//Vector3 curDir = avatar.position - footAvatar.position;
		Quaternion rot = Quaternion.FromToRotation(baseDir, curDir);
		float r = (target.position - thighAvatar.position).magnitude / baseLength;
		r = Mathf.Clamp(r, 0.0f, 1.0f);
		float deg = maxDeg * (1.0f - r);

		Vector3 offsetAngles = new Vector3(0.0f, bendAngle, deg) + initAngles;
		//Debug.Log("r:"+r);
		//Debug.Log("deg:"+deg);
		target.localRotation = rot * Quaternion.Euler(offsetAngles);
	}
}
