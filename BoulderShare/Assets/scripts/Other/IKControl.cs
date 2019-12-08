using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

public class IKControl : MonoBehaviour {
	
	protected Animator avatar;
	public bool ikActive = false;
	
	public Transform bodyObj = null;
	public Transform leftFootObj = null;
	public Transform rightFootObj = null;
	public Transform leftHandObj = null;
	public Transform rightHandObj = null;
	public Transform rightKnee = null;
	public Transform leftKnee = null;
	public Transform rightElbow = null;
	public Transform leftElbow = null;
	
	public float leftFootWeightPosition = 1;
	public float leftFootWeightRotation = 1;

	public float rightFootWeightPosition = 1;
	public float rightFootWeightRotation = 1;
	
	public float leftHandWeightPosition = 1;
	public float leftHandWeightRotation = 1;
	
	public float rightHandWeightPosition = 1;
	public float rightHandWeightRotation = 1;

	public float rightKneeWeightPosition = 1;

	public float leftKneeWeightPosition = 1;

	public float rightElbowWeightPosition = 1;

	public float leftElbowWeightPosition = 1;

	//public float lookAtWeight = 1.0f;
	
	// Use this for initialization
	void Start () {
		avatar = GetComponent<Animator>();
	}

	public void InitAvatar(){
		ikActive = false;
	}

	public bool GetIkActive(){
		return ikActive;
	}
/*
	public Transform[] GetPose(){
		Transform [] trans = new Transform[Enum.GetNames(typeof(AvatarControl.BODYS)).Length];

		trans[(int)AvatarControl.BODYS.BODY] = bodyObj;
		trans[(int)AvatarControl.BODYS.LF] = leftFootObj;
		trans[(int)AvatarControl.BODYS.RF] = rightFootObj;
		trans[(int)AvatarControl.BODYS.LH] = leftHandObj;
		trans[(int)AvatarControl.BODYS.RH] = rightHandObj;
		trans[(int)AvatarControl.BODYS.LK] = leftKnee;
		trans[(int)AvatarControl.BODYS.RK] = rightKnee;
		trans[(int)AvatarControl.BODYS.LE] = leftElbow;
		trans[(int)AvatarControl.BODYS.RE] = rightElbow;

		return trans;
	}*/

	public Vector3[] GetPosition(){
		Vector3[] pos = new Vector3[Enum.GetNames(typeof(AvatarControl.BODYS)).Length-1];
		
		pos[(int)AvatarControl.BODYS.BODY] = bodyObj.localPosition;
		pos[(int)AvatarControl.BODYS.LF] = leftFootObj.localPosition;
		pos[(int)AvatarControl.BODYS.RF] = rightFootObj.localPosition;
		pos[(int)AvatarControl.BODYS.LH] = leftHandObj.localPosition;
		pos[(int)AvatarControl.BODYS.RH] = rightHandObj.localPosition;
		pos[(int)AvatarControl.BODYS.LK] = leftKnee.localPosition;
		pos[(int)AvatarControl.BODYS.RK] = rightKnee.localPosition;
		pos[(int)AvatarControl.BODYS.LE] = leftElbow.localPosition;
		pos[(int)AvatarControl.BODYS.RE] = rightElbow.localPosition;

		return pos;

	}

	public Quaternion[] GetRotation(){
		Quaternion[] rot = new Quaternion[Enum.GetNames(typeof(AvatarControl.BODYS)).Length-1];
		
		rot[(int)AvatarControl.BODYS.BODY] = bodyObj.localRotation;
		rot[(int)AvatarControl.BODYS.LF] = leftFootObj.localRotation;
		rot[(int)AvatarControl.BODYS.RF] = rightFootObj.localRotation;
		rot[(int)AvatarControl.BODYS.LH] = leftHandObj.localRotation;
		rot[(int)AvatarControl.BODYS.RH] = rightHandObj.localRotation;
		rot[(int)AvatarControl.BODYS.LK] = leftKnee.localRotation;
		rot[(int)AvatarControl.BODYS.RK] = rightKnee.localRotation;
		rot[(int)AvatarControl.BODYS.LE] = leftElbow.localRotation;
		rot[(int)AvatarControl.BODYS.RE] = rightElbow.localRotation;

		return rot;

	}

	public void SetPose(Vector3[] pos, Quaternion[] rot){
		if(bodyObj != null ){
			bodyObj.localPosition = pos[(int)AvatarControl.BODYS.BODY];
			bodyObj.localRotation = rot[(int)AvatarControl.BODYS.BODY];
		}				

		if(leftFootObj != null ){
			leftFootObj.localPosition = pos[(int)AvatarControl.BODYS.LF];
			leftFootObj.localRotation = rot[(int)AvatarControl.BODYS.LF];
		}				
	
		if(rightFootObj != null ){
			rightFootObj.localPosition = pos[(int)AvatarControl.BODYS.RF];
			rightFootObj.localRotation = rot[(int)AvatarControl.BODYS.RF];
		}				

		if(leftHandObj != null ){
			leftHandObj.localPosition = pos[(int)AvatarControl.BODYS.LH];
			leftHandObj.localRotation = rot[(int)AvatarControl.BODYS.LH];
		}				
	
		if(rightHandObj != null ){
			rightHandObj.localPosition = pos[(int)AvatarControl.BODYS.RH];
			rightHandObj.localRotation = rot[(int)AvatarControl.BODYS.RH];
		}

		if (leftKnee != null ){
			leftKnee.localPosition = pos[(int)AvatarControl.BODYS.LK];
		}	
		if (rightKnee != null ){
			rightKnee.localPosition = pos[(int)AvatarControl.BODYS.RK];
		}
		if (leftElbow != null ){
			leftElbow.localPosition = pos[(int)AvatarControl.BODYS.LE];
		}
		if (rightElbow != null ){
			rightElbow.localPosition = pos[(int)AvatarControl.BODYS.RE];
		}
	}

	void OnAnimatorIK(int layerIndex){		
		if(avatar){
			if (ikActive){
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot,leftFootWeightPosition);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot,leftFootWeightRotation);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot,rightFootWeightPosition);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot,rightFootWeightRotation);

				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandWeightPosition);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftHandWeightRotation);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandWeightPosition);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightHand,rightHandWeightRotation);

				avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneeWeightPosition);

				avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneeWeightPosition);

				avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowWeightPosition);
				
				avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeightPosition);
				//avatar.SetLookAtWeight(lookAtWeight,0.3f,0.6f,1.0f,0.5f);
				
				if(bodyObj != null){
					avatar.bodyPosition = bodyObj.position;
					avatar.bodyRotation = bodyObj.rotation;
				}				

				if(leftFootObj != null){
					avatar.SetIKPosition(AvatarIKGoal.LeftFoot,leftFootObj.position);
					avatar.SetIKRotation(AvatarIKGoal.LeftFoot,leftFootObj.rotation);
				}				
			
				if(rightFootObj != null){
					avatar.SetIKPosition(AvatarIKGoal.RightFoot,rightFootObj.position);
					avatar.SetIKRotation(AvatarIKGoal.RightFoot,rightFootObj.rotation);
				}				

				if(leftHandObj != null){
					avatar.SetIKPosition(AvatarIKGoal.LeftHand,leftHandObj.position);
					avatar.SetIKRotation(AvatarIKGoal.LeftHand,leftHandObj.rotation);
				}				
			
				if(rightHandObj != null){
					avatar.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
					avatar.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
				}

				if (leftKnee != null){
					avatar.SetIKHintPosition(AvatarIKHint.LeftKnee,leftKnee.position);
				}	
				if (rightKnee != null){
					avatar.SetIKHintPosition(AvatarIKHint.RightKnee,rightKnee.position);
				}
				if (leftElbow != null){
					avatar.SetIKHintPosition(AvatarIKHint.LeftElbow,leftElbow.position);
				}
				if (rightElbow != null){
					avatar.SetIKHintPosition(AvatarIKHint.RightElbow,rightElbow.position);
				}				
			}else{
				//Debug.Log("ik false");
				avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot,0);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot,0);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot,0);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot,0);

				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
				avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand,0);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				avatar.SetIKRotationWeight(AvatarIKGoal.RightHand,0);

				avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
				avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);
				avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
				avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
							
				if(bodyObj != null){
					bodyObj.position = avatar.bodyPosition;
					bodyObj.rotation = avatar.bodyRotation;
				}				
				
				if(leftFootObj != null){
					leftFootObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
					leftFootObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
				}				
				
				if(rightFootObj != null){
					rightFootObj.position = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
					rightFootObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.RightFoot);
				}				
				
				if(leftHandObj != null){
					leftHandObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftHand);
					leftHandObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.LeftHand);
				}				
				
				if(rightHandObj != null){
					rightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);
					rightHandObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.RightHand);
				}

				if (leftKnee != null){
					leftKnee.position = avatar.GetIKHintPosition(AvatarIKHint.LeftKnee);
				}	
				if (rightKnee != null){
					rightKnee.position = avatar.GetIKHintPosition(AvatarIKHint.RightKnee);
				}
				if (leftElbow != null){
					leftElbow.position = avatar.GetIKHintPosition(AvatarIKHint.LeftElbow);
				}
				if (rightElbow != null){
					rightElbow.position = avatar.GetIKHintPosition(AvatarIKHint.RightElbow);
				}
				ikActive = true;
			}
		}
	}  
}
