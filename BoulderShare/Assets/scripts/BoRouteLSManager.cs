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
	private string dHScenesJson;
	private string dHoldsJson;

	private bool isLoaded;

	void Awake(){
		isLoaded = false;
	}
	public void LoadBoRoute(){
		//ボルートが選択されている場合（新規作成でない場合）
		GameObject obj = DontDestroyOnLoadManager.Get("BoRouteSelected");
		if (obj != null){
			Boroute b = obj.GetComponent<Boroute>();
			dRoute.ConstructionFromJson(b.GetDRoute());
			//holdsとscenesをファイルから読み込む
			string path = Application.persistentDataPath + Observer.ROUTEPATH 
				+dRoute.route.timestamp +"/";

			if (File.Exists(path + "holds.json")){
				dHoldsJson = File.ReadAllText(path + "holds.json");
			}

			if (File.Exists(path + "hscenes.json")){
				dHScenesJson = File.ReadAllText(path + "hscenes.json");
			}

			img = b.GetImg();
		
			/*
			dRouteData = dRoute.FromJsonToDRoute(b.GetBoRoute());
			//wall,Holds,ModelSizeはここでロードする
			wall.sprite = b.GetImg();
			dHolds.FromJson(dRouteData.holds);
			phase1.SetModelSize(dRouteData.scaleH2M);

			incline.SetIncline(dRouteData.incline);*/
			isLoaded = true;
			DontDestroyOnLoadManager.DestroyAll();
		}
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
			dRoute.Construction();
			//リストに追加
			if (isLoaded){
				//上書き保存
				for(int i = 0 ; i < list.Count ; i++){
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

			string filepath =  path + "/route.txt";
			string json = dRoute.ToJson();
			File.WriteAllText(filepath, json);

			//ホールドを書き込む
			filepath = path + "/holds.json";
			json = dHolds.ToJson();
			File.WriteAllText(filepath, json);
			//シーンを書き込む
			filepath = path + "/scenes.json";
			json = dHScenes.ToJson();
			File.WriteAllText(filepath, json);

			if (!isLoaded){
				filepath = Application.persistentDataPath + Observer.WALLPATH;
				string toPath = path + "/Wall.png";

           	 	if (File.Exists(filepath) && !File.Exists(toPath)){
            		File.Move(filepath, toPath);
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

/*
	private void OnApplicationPause(bool pauseStatus){

		if (pauseStatus){
			//アプリ中断
			//現在のボルートを一時フォルダに書き込む
			Write("Temp");
		}else{
			//アプリ復帰
			//一時フォルダの削除
			DeleteTemp();
		}
	}*/

	private void DeleteTemp(){
		string direcPath = Application.persistentDataPath + Observer.ROUTEPATH + "Temp" ;
		try{
			//tempディレクトリが存在する場合
			if (Directory.Exists(direcPath)){
				//画像ファイルを元に戻す
				string fromPath = direcPath + "/Wall.png";
				string toPath = Application.persistentDataPath + Observer.WALLPATH;
           	 	if (File.Exists(fromPath) && !File.Exists(toPath)){
            		File.Move(fromPath, toPath);
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
		dHolds.FromJson(dHoldsJson);

		dRoute.LoadFirst(); 
	}

	public void BoRouteLoadSecond(){
		dHScenes.FromJson(dHScenesJson);
	}

	public void BoRouteLoadThird(){
		dRoute.LoadThird();
	}
}
