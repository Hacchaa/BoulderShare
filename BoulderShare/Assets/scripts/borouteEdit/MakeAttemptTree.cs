using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeAttemptTree : MonoBehaviour
{
	public enum Mode {
		Loop,
		Edit,
		Add
	}
	[SerializeField] private GameObject rootMarks;
	private string[] twoDTouchMarks;
	private List<MyUtility.SceneCommentData3D> commentList;
	private Vector3[] positions;
	private Quaternion[] rotations;
	private bool isPoseSet = false;
	[SerializeField] private bool isWallMarkSet = false;
	[SerializeField] private HScenes2 hscenes;
	private HScene2 loadedScene;
	private int curIndex;
	private Mode mode;

	public void SetMode(Mode m){
		mode = m;
	}

	public Mode GetMode(){
		return mode;
	}

	public void SetIndex(int ind){
		curIndex = ind;
	}

	public int GetIndex(){
		return curIndex;
	}

	public void LoadScene(HScene2 scene){
		loadedScene = scene;
		twoDTouchMarks = scene.GetOnHolds();
		commentList = scene.GetComments();
		positions = scene.GetPose();
		rotations = scene.GetRots();
	}
	
	public void Init(){
		InitWallMarks();
		twoDTouchMarks = new string[0];
		commentList = new List<MyUtility.SceneCommentData3D>();
		positions = new Vector3[0];
		rotations = new Quaternion[0];
		isPoseSet = false;
		mode = Mode.Loop;
		loadedScene = null;
		curIndex = hscenes.GetNum() - 1;
	}

	private void InitWallMarks(){
		foreach(Transform t in rootMarks.transform){
			Destroy(t.gameObject);
		}		
		isWallMarkSet = false;
	}

	public void Make(){
		HScene2 scene;
		switch(mode){
			case Mode.Loop :
				scene = new HScene2();
				scene.SetOnHolds(twoDTouchMarks);
				scene.SaveComments(commentList);
				scene.SavePose(positions, rotations);
				hscenes.AddSceneLast(scene);
				break;
			case Mode.Edit :
				loadedScene.SetOnHolds(twoDTouchMarks);
				loadedScene.SaveComments(commentList);
				loadedScene.SavePose(positions, rotations);
				break;
			case Mode.Add :
				scene = new HScene2();
				scene.SetOnHolds(twoDTouchMarks);
				scene.SaveComments(commentList);
				scene.SavePose(positions, rotations);
				hscenes.AddSceneAt(scene, curIndex);
				break;
		}
	}

	public void Load(HScene2 scene){
		twoDTouchMarks = scene.GetOnHolds();
		commentList = scene.GetComments();
		positions = scene.GetPose();
		rotations = scene.GetRots();
		isPoseSet = true;
	}

	public bool IsPoseSet(){
		return isPoseSet;
	}

	public bool IsWallMarkSet(){
		return isWallMarkSet;
	}

	public GameObject GetWallMarks(){
		return rootMarks;
	}
	public void SetWallMarks(GameObject root){
		InitWallMarks();

		foreach(Transform t in root.transform){
			GameObject obj = new GameObject(t.name);
			obj.transform.SetParent(rootMarks.transform);
			obj.transform.localPosition = t.localPosition;
			obj.transform.localRotation = t.localRotation;
			obj.transform.localScale = t.localScale;
		}
		isWallMarkSet = true;
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
		isPoseSet = true;
	}

	public Quaternion[] GetRotations(){
		return rotations;
	}
}
