using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HScenes2 : MonoBehaviour {
	[SerializeField]
	private int curIndex;
	private List<HScene2> list ;
	private List<string> hScenesList;
	private List<string> failedList;
	[SerializeField]
	private AttemptTreeView atv;

	void Awake(){
		list = new List<HScene2>();
		curIndex = -1;
		hScenesList = new List<string>();
	}

	void Start(){
	}

	public void SetFailedList(List<string> list){
		failedList = new List<string>(list);
	}

	public List<string> GetFailedList(){
		return new List<string>(failedList);
	}


	public int GetNum(){
		return list.Count;
	}

	public int GetCurIndex(){
		return curIndex;
	}

	public List<HScene2> GetScenes(){
		return new List<HScene2>(list);
	}

	public HScene2 GetScene(int index){
		if (index < 0 || index > list.Count - 1){
			return null;
		}

		return list[index];
	}

	public void Construction(List<HScene2> data, int numOfCreatingHScene){
		list.Clear();
		list.AddRange(data);
		HScene2.SetNum(numOfCreatingHScene);
		if(list.Any()){
			curIndex = 0;
		}
	}


	public void AddScene(HScene2 hScene){
		curIndex++;
		list.Insert(curIndex, hScene);
	}

	//次に表示するべきhSceneを返す
	public HScene2 RemoveScene(){
		int nextIndex ;
		int n = list.Count;
		HScene2 next = null;

		if (n == 0){
			return null;
		}

		if (curIndex == 0){
			if (n == 1){
				next = null;
				nextIndex = -1;
			}else{
				next = list[1];
				nextIndex = 0;
			}
		}else{
			next = list[curIndex-1];
			nextIndex = curIndex - 1;
		}

		list.RemoveAt(curIndex);
		curIndex =  nextIndex ;
		return next;
	}

	public HScene2 NextScene(){
		if (curIndex < 0){
			return null;
		}
		if (curIndex < list.Count - 1){
			curIndex++;
		}
		return list[curIndex];
	}

	public HScene2 PrevScene(){
		if (curIndex < 0){
			return null;
		}
		if (curIndex > 0){
			curIndex--;
		}
		return list[curIndex];
	}

	public HScene2 GetCurScene(){
		if (curIndex < 0){
			return null;
		}
		return list[curIndex];
	}

	public string ToJson(){
		MyUtility.AttemptTree tree = new MyUtility.AttemptTree();
		tree.failedList = failedList;
		tree.numOfCreatingHScene = HScene2.GetNum();
		tree.data = new MyUtility.Scene[list.Count];
		int i = 0;
		foreach(HScene2 scene in list){
			MyUtility.Scene data = new MyUtility.Scene();
			data.id = scene.GetID();
			data.holdsOnHand = scene.GetOnHolds();
			data.comments = scene.GetComments();
			data.pose = scene.GetPose();
			data.rotate = scene.GetPRotate();

			tree.data[i] = data;
			i++;
		}
		return JsonUtility.ToJson(tree);
	}

	public void FromJson(string json){
		MyUtility.AttemptTree tree = JsonUtility.FromJson<MyUtility.AttemptTree>(json);
		list = new List<HScene2>();

		for(int i = 0 ; i < tree.data.Length ; i++){
			MyUtility.Scene data = tree.data[i];
			HScene2 scene = new HScene2();
			scene.SetID(data.id);
			scene.SetOnHolds(data.holdsOnHand);
			scene.SaveComments(data.comments);
			scene.SavePose(data.pose);
			scene.SavePRotate(data.rotate);
	
			list.Add(scene);
		}

		HScene2.SetNum(tree.numOfCreatingHScene);
		if (tree.data.Length > 0){
			curIndex = 0;
		}
		failedList = tree.failedList;
		atv.LoadingBorouteProc();
	}

	public void PrintJson(){
		Debug.Log(ToJson());
	}

	public string HScenesListToJson(){
		MyUtility.AttemptTrees trees = new MyUtility.AttemptTrees();
		RegistCurHScenes();
		trees.trees = new List<string>(hScenesList);
		return JsonUtility.ToJson(trees);
	}

	public void HScenesListFromJson(string json){
		MyUtility.AttemptTrees trees = JsonUtility.FromJson<MyUtility.AttemptTrees>(json);
		hScenesList = trees.trees;
		FromJson(GetLatestHScenes());
	}

	//hScenesをlistに登録
	//シリアライズ時とtryView.ReObs()で呼ばれる
	public void RegistCurHScenes(){
		hScenesList.Add(ToJson());
	}

	public string GetLatestHScenes(){
		int n = hScenesList.Count - 1;
		if (n >= 0){
			return hScenesList[n];
		}

		//空のjsonを返す
		return MakeEmptyJson();
	}

	public string MakeEmptyJson(){
		MyUtility.AttemptTree tree = new MyUtility.AttemptTree();
		tree.data = new MyUtility.Scene[0];
		tree.failedList = new List<string>();
		tree.numOfCreatingHScene = 0;

		return JsonUtility.ToJson(tree);
	}
}
