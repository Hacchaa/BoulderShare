using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

public class BoRouteLSManager : MonoBehaviour {
	public DRoute dRoute;
	private DRoute.Data dRouteData;
	private Sprite img;
	public DHScenes dHScenes;
	public DHolds dHolds;
	public Holds holds;
	public HScenes hScenes;
	public Post post;
	private string dHScenesJson;
	private string dHoldsJson;
	private string dRouteJson;
	private bool isLoaded;
	private bool isNew;
	[SerializeField]
	private List<string> placeList;

	void Awake(){
		isLoaded = false;
		isNew = false;
	}
	public void LoadBoRoute(){
		//必要な情報をBoRouteInfoオブジェから受け取る
		GameObject obj = DontDestroyOnLoadManager.Get("BoRouteInfo");
		if (obj != null){
			placeList = obj.GetComponent<BoRouteInfo>().GetPlaceList();
		}
		//ボルートが選択されている場合（新規作成でない場合）
		obj = DontDestroyOnLoadManager.Get("BoRouteSelected");
		if (obj != null){
			Boroute b = obj.GetComponent<Boroute>();
			dRoute.ConstructionFromJson(b.GetDRoute());
			dRouteJson = b.GetDRoute();
			img = b.GetImg();

			if(b.IsRouteTemp()){
				//一時保存の場合
				dHoldsJson = b.GetDHolds();
				dHScenesJson = b.GetDHScenes();
				
				//もし、タイムスタンプがnullの場合、（新規作成された一時保存の場合）
				if(String.IsNullOrEmpty(dRoute.route.timestamp)){
					isNew = true;
				}

				//一時保存を削除
				DeleteTemp();
			}else{
				//一時保存でない場合
				//holdsとscenesをファイルから読み込む
				string path = Application.persistentDataPath + Observer.ROUTEPATH 
					+dRoute.route.timestamp +"/";

				if (File.Exists(path + "holds.json")){
					dHoldsJson = File.ReadAllText(path + "holds.json");
				}

				if (File.Exists(path + "hscenes.json")){
					dHScenesJson = File.ReadAllText(path + "hscenes.json");
				}
			}
			/*
			dRouteData = dRoute.FromJsonToDRoute(b.GetBoRoute());
			//wall,Holds,ModelSizeはここでロードする
			wall.sprite = b.GetImg();
			dHolds.FromJson(dRouteData.holds);
			phase1.SetModelSize(dRouteData.scaleH2M);

			incline.SetIncline(dRouteData.incline);*/
			isLoaded = true;
		}else{
			isNew = true;
		}
		Debug.Log("isNew:"+isNew);
		Debug.Log("isLoaded:"+isLoaded);
		DontDestroyOnLoadManager.DestroyAll();
	}

	public void SaveBoRoute(){

		//dRouteListのjsonをファイルから読み込む
		string path = Application.persistentDataPath + Observer.ROUTEPATH;

		try{
			if (!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}

			string filepath =  path + "route.json";
			DRoute.DataArr dataArr = new DRoute.DataArr();
			List<DRoute.Data> list = new List<DRoute.Data>();
			string json;

			if (File.Exists(filepath)){
				//既にデータが保存してあるならば
				json = File.ReadAllText(filepath);
				dataArr = DRoute.ConvertJsonToDRouteList(json);
				list.AddRange(dataArr.arr);
			}
			//保存するdRouteを構成
			dRoute.Construction(!isNew);
			//リストに追加
			if (!isNew){
				//上書き保存
				int num = list.Count;
				for(int i = 0 ; i < num ; i++){
					if (list[i].timestamp.Equals(dRoute.route.timestamp)){
						list[i] = dRoute.route;
						break;
					}
				}
			}else{
				//新規作成
				list.Add(dRoute.route);
			}
			//dRoutelistをjsonに変換
			dataArr.arr = list.ToArray();
			json = DRoute.ConvertDRouteListToJson(dataArr);
			//ファイルに書き込む
			File.WriteAllText(filepath, json);

			//ホールドを書き込む
			path += dRoute.route.timestamp +"/";
			if (!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}
			filepath = path + "holds.json";
			json = dHolds.ToJson();
			File.WriteAllText(filepath, json);
			//シーンを書き込む
			filepath = path + "hscenes.json";
			json = dHScenes.ToJson();
			File.WriteAllText(filepath, json);

			//画像を保存する
			string wallpath = Application.persistentDataPath + Observer.WALLPATH;
			string toPath = path + "Wall.png";

           	if (File.Exists(wallpath) && !File.Exists(toPath)){
            	File.Move(wallpath, toPath);
			}

	    }catch (Exception e) {
	        Debug.Log("The process failed: " + e.ToString());
	        string p = Application.persistentDataPath + Observer.ROUTEPATH + dRoute.route.timestamp;
	        if (!Directory.Exists(p)){
				Directory.Delete(p, true);
			}
	    }
	}

	private void Write(string fileName){
		string path = Application.persistentDataPath + Observer.ROUTEPATH + fileName ;
		
		try{
			if (!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}

			//ボルート詳細を書き込む
			string filepath = path + "/route.json";
			string json = "";
			if (post.IsInit()){
				//初期化が住んでいる場合
				json = dRoute.ToJson(true);
			}else{
				//初期化が住んでいない場合

				//再編集かどうか
				if (!isLoaded){
					//再編集でない場合
					json = dRoute.GetEmptyJson();
				}else{
					//再編集の場合
					json = dRouteJson;
				}
			}
			File.WriteAllText(filepath, json);

			//ホールドを書き込む
			filepath = path + "/holds.json";
			if (holds.IsInit()){
				//初期化が住んでいる場合
				json = dHolds.ToJson();
			}else{
				//初期化が住んでいない場合

				//再編集かどうか
				if (!isLoaded){
					//再編集でない場合
					json = dHolds.GetEmptyJson();
				}else{
					//再編集の場合
					json = dHoldsJson;
				}
			}
			File.WriteAllText(filepath, json);

			//シーンを書き込む
			filepath = path + "/hscenes.json";
			if (hScenes.IsInit()){
				//初期化が住んでいる場合
				json = dHScenes.ToJson();
			}else{
				//初期化が住んでいない場合

				//再編集かどうか
				if (!isLoaded){
					//再編集でない場合
					json = dHScenes.GetEmptyJson();
				}else{
					//再編集の場合
					json = dHScenesJson;
				}
			}
			File.WriteAllText(filepath, json);
			
			//画像を書き込む
			if (isNew){
				filepath = Application.persistentDataPath + Observer.WALLPATH;
				string toPath = path + "/Wall.png";
           	 	if (File.Exists(filepath) && !File.Exists(toPath)){
            		File.Move(filepath, toPath);
            		//File.Delete(filepath);
            	}
	       	}
	    }catch (Exception e) {
	            Debug.Log("The process failed: " + e.ToString());
	    }
	}

	public Sprite GetImg(){
		return img;
	}

	public bool IsLoaded(){
		return isLoaded;
	}

	public bool IsNew(){
		return isNew;
	}

	public List<string> GetPlaceList(){
		return new List<string>(placeList);
	}

	public void SaveTemporary(){
		Write("Temp");
	}


	private void OnApplicationPause(bool pauseStatus){

		if (pauseStatus){
			//アプリ中断
			//現在のボルートを一時フォルダに書き込む
			SaveTemporary();
		}else{
			//アプリ復帰
			//一時フォルダの削除
			DeleteTemp();
		}
	}

	private void DeleteTemp(){
		string direcPath = Application.persistentDataPath + Observer.ROUTEPATH + "Temp" ;
		try{
			//tempディレクトリが存在する場合
			if (Directory.Exists(direcPath)){
				//画像ファイルを元に戻す
				string fromPath = direcPath + "/Wall.png";
				string toPath = Application.persistentDataPath + Observer.WALLPATH;
           	 	if (File.Exists(fromPath)){
            		File.Copy(fromPath, toPath, true);
            	}
            	//ディレクトリの削除
				Directory.Delete(direcPath, true);
			}
		}catch (Exception e){
			Debug.Log("The process failed: " + e.ToString());
		}
		
	}


	//オブジェクトの初期化処理の後ロードする
	public void BoRouteLoadFirst(){
		Debug.Log(dHoldsJson);
		dHolds.FromJson(dHoldsJson);

		dRoute.LoadFirst(); 
	}

	public void BoRouteLoadSecond(){
		Debug.Log(dHScenesJson);
		dHScenes.FromJson(dHScenesJson);
	}

	public void BoRouteLoadThird(){
		dRoute.LoadThird();
	}

	public void DeployPList(){
		dRoute.SetPList(placeList);
	}
}
