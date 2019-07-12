using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RootMotion.FinalIK;

public class FBBAimIKComponent : MonoBehaviour
{
	[SerializeField] private MyUtility.FullBodyMark targetAvatarBodyID;
	[SerializeField] private AimIK aimIK;
	[SerializeField] private FBBIKMarkAimed aimedMark;
	[SerializeField] private FBBIKPoleAimed pole;
	[SerializeField] private HumanModel humanModel;

	bool isActive = false;
	private Transform avatar;
	private Quaternion storedRot;
	private Quaternion initStoredRot;

	public void Init(){
		aimedMark.Init();
		if(pole != null){
			pole.Init();
		}

		isActive = false;
		initStoredRot = avatar.localRotation;
	}
	public void AddOnPostBeginDragAction(Action a){
		aimedMark.AddOnPostBeginDragAction(a);
		pole.AddOnPostBeginDragAction(a);
	}
	public void RemoveOnPostBeginDragAction(Action a){
		aimedMark.RemoveOnPostBeginDragAction(a);
		pole.RemoveOnPostBeginDragAction(a);
	}
	public Quaternion GetRotation(){
		return storedRot;
	}

	public void SetRotation(Quaternion rot){
		storedRot = rot;
	}

	public void DeterminePolePosition(){
		pole.DeterminePosition(aimIK.solver.poleAxis);
	}

	public void Reset(){
		Deactivate();
		storedRot = initStoredRot;
	}

	public void Activate(){
		isActive = true;
		float size = humanModel.GetModelSize();
		aimedMark.StoreModelSize(size);
		aimedMark.DeterminePosition(aimIK.solver.axis);
		aimedMark.Activate();

		if (pole != null){
			pole.StoreModelSize(size);
			pole.DeterminePosition(aimIK.solver.poleAxis);
			pole.Activate();
		}
	}

	public void Deactivate(){
		isActive = false;

		storedRot = avatar.localRotation;

		aimedMark.Deactivate();
		if (pole != null){
			pole.Deactivate();
		}
	}

	public bool IsActive(){
		return isActive;
	}

	public MyUtility.FullBodyMark GetTargetAvatarBodyID(){
		return targetAvatarBodyID;
	}

	public void SetCamera(Camera cam){
		aimedMark.SetCamera(cam);
		if (pole != null){
			pole.SetCamera(cam);
		}
	}

	public void SetAvatar(Transform a){
		aimedMark.SetAvatar(a);
		if (pole != null){
			pole.SetAvatar(a);
		}

		avatar = a;
		storedRot = avatar.localRotation;
	}

	public void UpdateManually(){
		if (isActive){
			if (aimIK != null){
				aimIK.GetIKSolver().Update();
			}
		}else{
			avatar.localRotation = storedRot;
		}
	}

}
