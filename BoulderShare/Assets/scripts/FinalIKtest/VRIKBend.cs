using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RootMotion.FinalIK;
public class VRIKBend : VRIKComponent {
	[SerializeField] private float r = 0.5f;
	[SerializeField] private Transform centerAvatar;
	[SerializeField] private LineRenderer line;


	void Update(){
		if (gameObject.activeSelf){
		    Vector3[] p = new Vector3[2];
	    	p[0] = faceAvatar.position;
	    	p[1] = transform.position;
	    	line.SetPositions(p);
        }
        if (centerAvatar != null){
			Vector3 dir = rootVRMark.InverseTransformPoint(target.position) - rootVRMark.InverseTransformPoint(centerAvatar.position);
			if (dir.magnitude > r){
				target.localPosition = rootVRMark.InverseTransformPoint(centerAvatar.position) + dir.normalized * r;
			}
		}
	}
	public override void Show(){
		render.enabled = true;
		line.gameObject.SetActive(true);
	}
	public override void Hide(){
		render.enabled = false;
		line.gameObject.SetActive(false);
	}
	//avatarの位置に移動させる
	public override void Correct(){
	}
}
