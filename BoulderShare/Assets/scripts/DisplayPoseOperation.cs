using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayPoseOperation : MonoBehaviour{
	public GameObject pose;
	public GameObject display;
	public IKControl avatarDisplay;
	public IKControl avatarPose;
	public Hold_Pose hp;
	public TransformObj[] avatarControlObj ;
	// Use this for initialization
	void Start () {
		
	}

	IEnumerator Proc(){
		pose.SetActive(true);
		while(!avatarPose.GetIkActive()){
			yield return null;
		}
		hp.Sync();
		Transform[] poses = avatarDisplay.GetPose();
		Vector3[] holdPos = hp.GetHoldsPos();

		if (holdPos[(int)SceneFocus.Choice.RH] != Vector3.zero){
			if (!avatarControlObj[0].IsFixed()){
				poses[(int)AvatarControl.BODYS.RH].localPosition 
					= holdPos[(int)SceneFocus.Choice.RH];
			}
			avatarControlObj[0].SetFixed(true);
			
		}else{
			avatarControlObj[0].SetFixed(false);
		}
		if (holdPos[(int)SceneFocus.Choice.RF] != Vector3.zero){
			if (!avatarControlObj[1].IsFixed()){
				poses[(int)AvatarControl.BODYS.RF].localPosition 
					= holdPos[(int)SceneFocus.Choice.RF];
			}
			avatarControlObj[1].SetFixed(true);
			
		}else{
			avatarControlObj[1].SetFixed(false);
		}
		if (holdPos[(int)SceneFocus.Choice.LH] != Vector3.zero){
			if (!avatarControlObj[2].IsFixed()){
				poses[(int)AvatarControl.BODYS.LH].localPosition 
					= holdPos[(int)SceneFocus.Choice.LH];
			}
			avatarControlObj[2].SetFixed(true);
			
		}else{
			avatarControlObj[2].SetFixed(false);
		}
		if (holdPos[(int)SceneFocus.Choice.LF] != Vector3.zero){
			if (!avatarControlObj[3].IsFixed()){
				poses[(int)AvatarControl.BODYS.LF].localPosition 
					= holdPos[(int)SceneFocus.Choice.LF];
			}
			avatarControlObj[3].SetFixed(true);
			
		}else{
			avatarControlObj[3].SetFixed(false);
		}

		avatarPose.SetPose(poses);
		display.SetActive(false);
	}

	public void Open(){
		StartCoroutine("Proc");
	}
}
