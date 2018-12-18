using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

public class IKControl2 : MonoBehaviour {
	
	protected Animator avatar;
	public bool ikActive = false;
	
	public Transform lookObj = null;
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

	[SerializeField]
	private bool isLookingActive;
	//public float lookAtWeight = 1.0f;
	
	// Use this for initialization
	void Start () {
		avatar = GetComponent<Animator>();
	}

	public void InitAvatar(){
		ikActive = false;
		isLookingActive = false;
	}

	public void SetIsLookingActivate(bool b){
		isLookingActive = b;
	}

	public bool IsLookingActivate(){
		return isLookingActive;
	}

	public bool IsIKActive(){
		return ikActive;
	}

	public Vector3 GetBodyPosition(){
		return bodyObj.position;
	}

	public Vector3[] GetPosition(){
		Vector3[] pos = new Vector3[Enum.GetNames(typeof(EditorManager.BODYS)).Length-1];
		
		pos[(int)EditorManager.BODYS.BODY] = bodyObj.localPosition;
		pos[(int)EditorManager.BODYS.LF] = leftFootObj.localPosition;
		pos[(int)EditorManager.BODYS.RF] = rightFootObj.localPosition;
		pos[(int)EditorManager.BODYS.LH] = leftHandObj.localPosition;
		pos[(int)EditorManager.BODYS.RH] = rightHandObj.localPosition;
		pos[(int)EditorManager.BODYS.LK] = leftKnee.localPosition;
		pos[(int)EditorManager.BODYS.RK] = rightKnee.localPosition;
		pos[(int)EditorManager.BODYS.LE] = leftElbow.localPosition;
		pos[(int)EditorManager.BODYS.RE] = rightElbow.localPosition;

		pos[(int)EditorManager.BODYS.LOOK] = lookObj.localPosition;

		return pos;
	}

	public Quaternion[] GetRotation(){
		Quaternion[] rot = new Quaternion[Enum.GetNames(typeof(EditorManager.BODYS)).Length-1];
		
		rot[(int)EditorManager.BODYS.BODY] = bodyObj.localRotation;
		rot[(int)EditorManager.BODYS.LF] = leftFootObj.localRotation;
		rot[(int)EditorManager.BODYS.RF] = rightFootObj.localRotation;
		rot[(int)EditorManager.BODYS.LH] = leftHandObj.localRotation;
		rot[(int)EditorManager.BODYS.RH] = rightHandObj.localRotation;
		rot[(int)EditorManager.BODYS.LK] = leftKnee.localRotation;
		rot[(int)EditorManager.BODYS.RK] = rightKnee.localRotation;
		rot[(int)EditorManager.BODYS.LE] = leftElbow.localRotation;
		rot[(int)EditorManager.BODYS.RE] = rightElbow.localRotation;

		return rot;

	}

	public void SetPose(Vector3[] pos, Quaternion[] rot){
		if(bodyObj != null ){
			bodyObj.localPosition = pos[(int)EditorManager.BODYS.BODY];
			bodyObj.localRotation = rot[(int)EditorManager.BODYS.BODY];
		}				

		if(leftFootObj != null ){
			leftFootObj.localPosition = pos[(int)EditorManager.BODYS.LF];
			leftFootObj.localRotation = rot[(int)EditorManager.BODYS.LF];
		}				
	
		if(rightFootObj != null ){
			rightFootObj.localPosition = pos[(int)EditorManager.BODYS.RF];
			rightFootObj.localRotation = rot[(int)EditorManager.BODYS.RF];
		}				

		if(leftHandObj != null ){
			leftHandObj.localPosition = pos[(int)EditorManager.BODYS.LH];
			leftHandObj.localRotation = rot[(int)EditorManager.BODYS.LH];
		}				
	
		if(rightHandObj != null ){
			rightHandObj.localPosition = pos[(int)EditorManager.BODYS.RH];
			rightHandObj.localRotation = rot[(int)EditorManager.BODYS.RH];
		}

		if (leftKnee != null ){
			leftKnee.localPosition = pos[(int)EditorManager.BODYS.LK];
		}	
		if (rightKnee != null ){
			rightKnee.localPosition = pos[(int)EditorManager.BODYS.RK];
		}
		if (leftElbow != null ){
			leftElbow.localPosition = pos[(int)EditorManager.BODYS.LE];
		}
		if (rightElbow != null ){
			rightElbow.localPosition = pos[(int)EditorManager.BODYS.RE];
		}

		if (lookObj != null){
			lookObj.localPosition = pos[(int)EditorManager.BODYS.LOOK];
		}
	}

	void OnAnimatorIK(int layerIndex){		
		if(avatar){
			if (ikActive){
				if (isLookingActive){
					avatar.SetLookAtWeight(1.0f, 0.0f, 1.0f, 0.0f, 0.5f);
				}

				avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot,leftFootWeightPosition);
				//avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot,leftFootWeightRotation);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot,rightFootWeightPosition);
				//avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot,rightFootWeightRotation);

				avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandWeightPosition);
				//avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftHandWeightRotation);
							
				avatar.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandWeightPosition);
				//avatar.SetIKRotationWeight(AvatarIKGoal.RightHand,rightHandWeightRotation);

				//avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftKneeWeightPosition);

				//avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightKneeWeightPosition);

				//avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowWeightPosition);
				
				//avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeightPosition);
				//avatar.SetLookAtWeight(lookAtWeight,0.3f,0.6f,1.0f,0.5f);
				

				if(isLookingActive && lookObj != null){
					avatar.SetLookAtPosition(lookObj.position);
				}

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
				avatar.SetLookAtWeight(0.0f);
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
