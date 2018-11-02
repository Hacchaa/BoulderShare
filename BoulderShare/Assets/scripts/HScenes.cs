using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HScenes : MonoBehaviour {
	public Text curNum;
	public Text num;
	private List<HScene> list ;
	public SceneFocus sf;
	public SceneNum sn;
	public Transform holds;
	public CommentScroll cs;
	public IKControl ik;
	public AvatarControl ac;
	public BoRouteLSManager bManager;

	void Awake(){
		list = new List<HScene>();
		curNum.text = "0";
		num.text = "0";
	}
	void Start(){
		if (bManager.IsLoaded()){
			bManager.BoRouteLoadSecond();
		}else{
			InitScenes();
		}
	}

	public int GetNum(){
		return int.Parse(num.text);
	}

	public List<HScene> GetScenes(){
		return list;
	}

	public void Construction(List<HScene> data){
		int n = data.Count;
		Awake();
		sn.Init();
		
		for(int i = 0 ; i < n ; i++){
			sn.Add(0);
		}

		list = data;
		curNum.text = "1";
		num.text = n + "";
		LoadScene(list[0]);
	}

	public void InitScenes(){
		sn.Init();
		Awake();
		AddScene();
	}

	private void LoadScene(HScene scene){
		sf.Reset();
		ac.Init();
		string[] onHolds = scene.GetOnHolds();
		//Debug.Log("onholds:"+onHolds);
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if (!string.IsNullOrEmpty(onHolds[i])){
				Transform t = holds.Find(onHolds[i]);
				if (t != null){
					Hold hold = t.gameObject.GetComponent<Hold>();
					sf.SetFocusHold(i, hold);
					ac.SetFixed(i, true);
				}
			}
		}
		cs.SetComments(scene.GetComments());
		if (scene.IsPose()){
			ik.SetPose(scene.GetPose(), scene.GetPRotate());
		}else{
			ik.InitAvatar();
		}
	}

	public Hold[] GetCurHolds(){
		return sf.GetFocusHold();
	}

	//現在シーンを保存する
	public void Save(){
		int index = int.Parse(curNum.text) - 1;
		SaveScene(list[index]);
	}

	private void SaveScene(HScene scene){
		scene.SaveOnHolds(sf.GetFocusHold());
		scene.SaveComments(cs.GetComments());
		scene.SavePose(ik.GetPosition());
		scene.SavePRotate(ik.GetRotation());
	}


	public void AddScene(){
		int index = int.Parse(curNum.text) - 1;
		HScene scene = new HScene();

		if (index >= 0){
			SaveScene(list[index]);
		}

		LoadScene(scene);
		list.Insert(index+1, scene);
		sn.Add(index+1);

		curNum.text = index + 2 + "";
		num.text = int.Parse(num.text) + 1 + "";
	}

	public void RemoveScene(){
		int index = int.Parse(curNum.text) - 1;
		int nextIndex ;
		int n = int.Parse(num.text);
		HScene next ;
		if (n == 1){
			next = new HScene();
			list.Add(next);
			sn.Add(1);
			nextIndex = 1;
		}else if (index == 0){
			next = list[1];
			nextIndex = 1;
		}else{
			next = list[index-1];
			nextIndex = index ;
		}

		list.RemoveAt(index);
		LoadScene(next);
		sn.Remove(index);

		curNum.text = nextIndex + "";
		num.text = list.Count + "";
	}

	public void NextScene(){
		int index = int.Parse(curNum.text) - 1;

		if (index+1 < list.Count){
			SaveScene(list[index]);
			LoadScene(list[index+1]);
			sn.Next();

			curNum.text = index + 2 + "";
		}
	}

	public void PrevScene(){
		int index = int.Parse(curNum.text) - 1;

		if (index > 0){
			SaveScene(list[index]);
			LoadScene(list[index-1]);
			sn.Prev();

			curNum.text = index + "";
		}
	}

	public void NextScene2(){
		int index = int.Parse(curNum.text) - 1;

		if (index+1 < list.Count){
			SaveScene(list[index]);
			LoadScene(list[index+1]);
			sn.Next();

			curNum.text = index + 2 + "";
		}else if (index+1 == list.Count){
			AddScene();
		}
	}

	public void PrevScene2(){
		int index = int.Parse(curNum.text) - 1;

		if (index > 0){
			SaveScene(list[index]);
			LoadScene(list[index-1]);
			sn.Prev();

			curNum.text = index + "";
		}else if(index == 0){
			HScene newScene = new HScene();

			SaveScene(list[index]);
			LoadScene(newScene);
			list.Insert(index, newScene);
			sn.Add(0);

			curNum.text = 1 + "";
			num.text = list.Count + "";
		}
	}
	
}
