using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HScenes : MonoBehaviour {
	public Text curNum;
	public Text num;
	[SerializeField]
	private List<HScene> list ;
	public SceneFocus sf;
	public SceneNum sn;
	public Transform holds;
	public CommentScroll cs;
	public IKControl ik;
	public AvatarControl ac;
	public BoRouteLSManager bManager;
	private bool isInit = false;
	private string[] curHoldsInPose;
	private bool isCurScenePosable = false;

	void Awake(){
		curHoldsInPose = new string[4];
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
		isInit = true;
	}


	//use 3DModel/Pose/Canvas_Pose/Frame/Studio/ApplyButton
	public void AdmitSettingPose(){
		int index = int.Parse(curNum.text) - 1;
		list[index].SetIsPoseSaved(true);

		Hold[] arr = sf.GetFocusHold();
		for(int i = 0 ; i < arr.Length ; i++){
			if (arr[i] != null){
				curHoldsInPose[i] = arr[i].gameObject.name;
			}else{
				curHoldsInPose[i] = "" ;
			}
		}
		isCurScenePosable = true;
	}

	public string[] GetCurHoldsInPose(){
		return curHoldsInPose;
	}

	public bool IsInit(){
		return isInit;
	}
	public bool IsCurScenePosable(){
		return isCurScenePosable;
	}

	public int GetNum(){
		return int.Parse(num.text);
	}

	public List<HScene> GetScenes(){
		return list;
	}

	public void Construction(List<HScene> data, int numOfCreatingHScene){
		int n = data.Count;
		Awake();
		sn.Init();
		
		for(int i = 0 ; i < n ; i++){
			sn.Add(0);
		}

		list = data;
		HScene.SetNum(numOfCreatingHScene);
		curNum.text = "1";
		num.text = n + "";
		LoadScene(list[0]);
	}

	public void InitScenes(){
		sn.Init();
		AddScene();
	}

	private void LoadScene(HScene scene){
		sf.Reset();
		ac.Init();
		string[] onHolds = scene.GetOnHolds();
		onHolds.CopyTo(curHoldsInPose, 0);
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
		isCurScenePosable = scene.IsPose();
	}

	public Hold[] GetCurHolds(){
		return sf.GetFocusHold();
	}

	public string[] GetCurHolds2(){
		Hold[] h = sf.GetFocusHold();
		string[] s = new string[4];

		for(int i = 0 ; i < s.Length ; i++){
			if (h[i] != null){
				s[i] = h[i].gameObject.name;
			}
		}

		return s;
	}

	//現在シーンを保存する
	public void Save(){
		int index = int.Parse(curNum.text) - 1;
		SaveScene(list[index]);
	}

	private void SaveScene(HScene scene){
		//もしhsceneの内容が変更されていたら、hsceneを更新して新しいidを付与する
		if (!scene.IsEqualTo(sf.GetFocusHold(), ik.GetPosition(), ik.GetRotation())){
			//Debug.Log("curScene"+scene.GetID() + "is Overwriting");
			//初めて保存するとき以外のみ新しいidを付与
			if (scene.IsAlreadySaved()){
				//Debug.Log("ID of curScene was updated("+scene.GetID() +" to "+ HScene.GetNum());
				scene.SetID(HScene.GetNum());
				HScene.SetNum(HScene.GetNum()+1);
			}
			scene.SaveOnHolds(sf.GetFocusHold());
			//このシーンをロードしてから、一回でもポーズを保存した場合
			if (scene.IsPoseSaved()){
				scene.SavePose(ik.GetPosition());
				scene.SavePRotate(ik.GetRotation());
			}
		}
		//コメントは必ず上書きする
		scene.SaveComments(cs.GetComments());
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
