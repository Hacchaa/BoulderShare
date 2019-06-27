using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HScenes2 : MonoBehaviour {
	[SerializeField] private int curIndex;
	private List<HScene2> list ;
	private List<MyUtility.AttemptTree> atList;
	private Dictionary<int, HScene2> masterScenes;

	[SerializeField] private int num;
	[SerializeField] private int sceneNum;
	[SerializeField] private WallManager wallManager;

	public void Init(){
		list = new List<HScene2>();
		curIndex = -1;
		atList = new List<MyUtility.AttemptTree>();
		masterScenes = new Dictionary<int, HScene2>();
		num = -1;
		sceneNum = -1;
	}

	public bool IsATListEmpty(){
		return !atList.Any();
	}
	public bool IsATEmpty(){
		return !list.Any();
	}

	public void InitAT(){
		list.Clear();
		curIndex = -1;
	}

	void Update(){
		if (atList != null){
			num = atList.Count;
		}

		if (list != null){
			sceneNum = list.Count;
		}
	}

	public int GetNum(){
		return list.Count;
	}

	public int GetCurIndex(){
		return curIndex;
	}

	public void SetCurIndex(int index){
		if (index < 0 || index > list.Count - 1){
			curIndex = -1;
			return ;
		}
		curIndex = index ;
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


	public void AddScene(HScene2 hScene){
		curIndex++;
		list.Insert(curIndex, hScene);

	}

	public void AddSceneAt(HScene2 hScene, int index){
		if (index < 0 || index > list.Count){
			return ;
		}		
		list.Insert(index, hScene);
	}

	public void AddSceneLast(HScene2 hScene){
		if (hScene != null){
			list.Add(hScene);
			curIndex = list.Count - 1 ;
		}
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
	public HScene2 NextSceneWithLoop(){
		curIndex++;
		if (curIndex < 0 || curIndex > list.Count - 1){
			curIndex = 0;
		}
		return list[curIndex];
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

	public void SetATList(List<MyUtility.AttemptTree> l){
		atList = new List<MyUtility.AttemptTree>(l);
	}

	public List<MyUtility.AttemptTree> GetATList(){
		return new List<MyUtility.AttemptTree>(atList);
	}

	public MyUtility.AttemptTree GetAT(int index){
		return atList[index];
	}

	public void LoadMasterScenes(List<MyUtility.Scene> mList){
		masterScenes.Clear();
		foreach(MyUtility.Scene data in mList){
			HScene2 scene = new HScene2();
			scene.SetID(data.id);
			scene.SetOnHolds(data.holdsOnHand);
			scene.SaveComments(data.comments);
			scene.SavePose(data.pose, data.rots, (FBBIKController.HandAnim)data.rightHandAnim, (FBBIKController.HandAnim)data.leftHandAnim);
			scene.SetFailureList(data.failureList);

			masterScenes.Add(scene.GetID(), scene);
		}
	}

	public List<MyUtility.Scene> GetMasterScenes(){
		MyUtility.Scene data;
		List<MyUtility.Scene> l = new List<MyUtility.Scene>();

		foreach(HScene2 scene in masterScenes.Values){
			data = new MyUtility.Scene();

			data.id = scene.GetID();
			data.holdsOnHand = scene.GetOnHolds();
			data.comments = scene.GetComments();
			data.pose = scene.GetPose();
			data.rots = scene.GetRots();
			data.failureList = scene.GetFailureList();
			data.rightHandAnim = (int)scene.GetRightHandAnim();
			data.leftHandAnim = (int)scene.GetLeftHandAnim();

			l.Add(data);
		}

		return l;
	}
/*
	public string ToJson(){
		MyUtility.AttemptTree tree = new MyUtility.AttemptTree();
		tree.numOfCreatingHScene = HScene2.GetNum();
		tree.data = new MyUtility.Scene[list.Count];
		int i = 0;
		foreach(HScene2 scene in list){
			MyUtility.Scene data = new MyUtility.Scene();
			data.id = scene.GetID();
			data.holdsOnHand = scene.GetOnHolds();
			data.comments = scene.GetComments();
			data.pose = scene.GetPose();
			data.rots = scene.GetRots();
			data.isLookingActivate = scene.IsLookingActivate();

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
			scene.SavePose(data.pose, data.rots);
			scene.SetIsLookingActivate(data.isLookingActivate);
	
			list.Add(scene);
		}

		HScene2.SetNum(tree.numOfCreatingHScene);
		if (tree.data.Length > 0){
			curIndex = 0;
		}
		isModified = false;
	}
*/

	
	public Dictionary<int, MyUtility.Scene> GetConvertedScenes(){
		Dictionary<int, MyUtility.Scene> map = new Dictionary<int, MyUtility.Scene>();

		foreach(MyUtility.Scene scene in GetMasterScenes()){
			map.Add(scene.id, scene);
		}
		return map;
	}

/*
	public void PrintJson(){
		Debug.Log(ToJson());
	}

	public string atListToJson(){
		MyUtility.AttemptTrees trees = new MyUtility.AttemptTrees();
		RegistCurHScenes();
		trees.trees = new List<string>(atList);
		return JsonUtility.ToJson(trees);
	}

	public void atListFromJson(string json){
		MyUtility.AttemptTrees trees = JsonUtility.FromJson<MyUtility.AttemptTrees>(json);
		atList = trees.trees;
		FromJson(GetLatestHScenes());
	}*/

	//hScenesをlistに登録
	//シリアライズ時とtryView.ReObs()で呼ばれる
	public void RegistCurHScenes(){
		List<int> ord = new List<int>();
		MyUtility.AttemptTree tree = new MyUtility.AttemptTree();

		Debug.Log("registcurhscnes");
		
		foreach(HScene2 scene in list){
			ord.Add(scene.GetID());
			if(masterScenes.ContainsKey(scene.GetID())){
				Debug.Log("overwrite "+scene.GetID());
				//masterを上書きする
				masterScenes[scene.GetID()] = scene;
			}else{
				Debug.Log("add "+scene.GetID());
				//追加
				masterScenes.Add(scene.GetID(), scene);
			}
		}

		//atListの更新
		tree.idList = ord;
		tree.numOfCreatingHScene = HScene2.GetNum();
		tree.marks = wallManager.GetMarks();
		atList.Add(tree);
	}

	public int GetATNum(){
		return atList.Count;
	}

	public void LoadHScenes(int index){
		InitAT();
		if (index < 0 || index > atList.Count - 1){
			return ;
		}
		curIndex = 0;
		HScene2.SetNum(atList[index].numOfCreatingHScene);
		wallManager.LoadMarks(atList[index].marks);
		foreach(int id in atList[index].idList){
			HScene2 master = masterScenes[id];
			HScene2 scene = new HScene2();
			scene.SetID(master.GetID());
			scene.SetOnHolds(master.GetOnHolds());
			scene.SaveComments(master.GetComments());
			scene.SavePose(master.GetPose(), master.GetRots(), master.GetRightHandAnim(), master.GetLeftHandAnim());
			scene.SetFailureList(master.GetFailureList());	

			list.Add(scene);	
		}
	}

	public void LoadLatestAT(){
		InitAT();
		int n = atList.Count - 1;
		if (n >= 0){
			LoadHScenes(n);
		}
	}
/*
	public string GetLatestAT(){
		int n = atList.Count - 1;
		if (n >= 0){
			return atList[n];
		}

		//空のjsonを返す
		return MakeEmptyJson();
	}

	public string MakeEmptyJson(){
		MyUtility.AttemptTree tree = new MyUtility.AttemptTree();
		tree.data = new MyUtility.Scene[0];
		tree.numOfCreatingHScene = 0;

		return JsonUtility.ToJson(tree);
	}*/
}
