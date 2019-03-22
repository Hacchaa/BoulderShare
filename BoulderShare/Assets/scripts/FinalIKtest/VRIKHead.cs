using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RootMotion.FinalIK;

public class VRIKHead : VRIKComponent{
	private Vector3 basePos;
	[SerializeField] private HumanModel humanModel;
	public override void Init(){
		basePos = target.position;
		ResetPos();
	}

	public override void ModifyPosition(){
		//target.position = transform.position;
	}

	protected override void OnPostEndDrag(){
		humanModel.SetCamAxisAsModelPos();
	}
}
