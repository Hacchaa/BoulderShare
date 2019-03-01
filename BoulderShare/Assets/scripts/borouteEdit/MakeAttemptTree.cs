using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeAttemptTree : MonoBehaviour
{
	private string[] twoDTouchMarks;
	private List<MyUtility.SceneCommentData3D> commentList;
	private Vector3[] positions;
	private Quaternion[] rotations;
	private bool isSet = false;
	[SerializeField] private HScenes2 hscenes;
	
	public void Init(){
		twoDTouchMarks = new string[0];
		commentList = new List<MyUtility.SceneCommentData3D>();
		positions = new Vector3[0];
		rotations = new Quaternion[0];
		isSet = false;
	}

	public void Make(){
		HScene2 scene = new HScene2();
		scene.SetOnHolds(twoDTouchMarks);
		scene.SaveComments(commentList);
		scene.SavePose(positions, rotations);

		hscenes.AddSceneLast(scene);
	}

	public void Load(HScene2 scene){
		twoDTouchMarks = scene.GetOnHolds();
		commentList = scene.GetComments();
		positions = scene.GetPose();
		rotations = scene.GetRots();
		isSet = true;
	}

	public bool IsPoseSet(){
		return isSet;
	}

	public string[] Get2DTouchMarks(){
		return twoDTouchMarks;
	}

	public void Set2DTouchMarks(string[] str){
		twoDTouchMarks = str;
	}

	public List<MyUtility.SceneCommentData3D> GetComments(){
		return commentList;
	}

	public void SetComments(List<MyUtility.SceneCommentData3D> list){
		commentList = new List<MyUtility.SceneCommentData3D>(list);
	}

	public Vector3[] GetPositions(){
		return positions;
	}

	public void SetPose(Vector3[] arr, Quaternion[] rot){
		positions = arr;
		rotations = rot;
		isSet = true;
	}

	public Quaternion[] GetRotations(){
		return rotations;
	}
}
