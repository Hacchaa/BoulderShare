using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

public class BorouteLSManager2 : MonoBehaviour {
	[SerializeField] private EditorManager eManager;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private TwoDWall twoDWall;
	private bool isNew;

	void Awake(){
		isNew = true;
	}
	public void WriteWallImage(){
		MyUtility.WriteImage(wallManager.GetMasterWallImage(), Application.persistentDataPath + EditorManager.WALLPATH + "Wall.png");
	}
	public void LoadBoroute(){
		//必要な情報をBoRouteInfoオブジェから受け取る
		GameObject obj = DontDestroyOnLoadManager.Get("InfoFromViewToEdit");
		InfoFromViewToEdit info = null;

		info = obj.GetComponent<InfoFromViewToEdit>();
		isNew = info.IsNew();
		
		//ボルートが選択されている場合（新規作成でない場合
		if (!isNew){
			//ボルートを読み込む
			string path = Application.persistentDataPath + EditorManager.BOROUTEPATH + info.GetDirName() + "/boroute.json";
			if(File.Exists(path)){
				string json = File.ReadAllText(path);
				BorouteFromJson(json);
			}
			//画像を読み込む
			path = Application.persistentDataPath + EditorManager.BOROUTEPATH + info.GetDirName() + "/Wall.png";
			if (File.Exists(path)){
				wallManager.CommitWallImage(MyUtility.LoadImage(path));
			}

			//読み込まれたボルートが一時保存ならば、新規作成扱いにする
			if(info.GetDirName().Equals("temp")){
				isNew = true;
			}
			
		}
		DeleteTemp();
		Debug.Log("isNew:"+isNew);
		DontDestroyOnLoadManager.DestroyAll();
	}

	public void SaveBoroute(bool isTemp = false){
		string path = Application.persistentDataPath + EditorManager.BOROUTEPATH;
		string ts = "";
		try{
			if (!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}

			//ボルートを保存する
			string json = BorouteToJson();
			ts = eManager.GetTimestamp();
			string dicPath = path;
			if (isTemp){
				dicPath += "temp/";
			}else{
				dicPath += ts + "/";
			}

			if (!Directory.Exists(dicPath)){
				Directory.CreateDirectory(dicPath);
			}
			string filepath = dicPath + "boroute.json";
			File.WriteAllText(filepath, json);

			//画像を保存する
			string wallpath = Application.persistentDataPath + EditorManager.WALLPATH + "Wall.png";
			string toPath = dicPath + "Wall.png";

           	if (File.Exists(wallpath)){
            	File.Copy(wallpath, toPath, true);
			}
			//サムネイル画像を保存
        	toPath = dicPath + "thumbnail.png";
        	File.Delete(toPath);
        	WriteThumbnail(toPath);

        	//一時保存でない場合
			if (!isTemp){
				//検索用データ構造を書き込む
				filepath =  path + "search.json";
				List<MyUtility.BorouteInformation> list;

				if (File.Exists(filepath)){
					//既にデータが保存してあるならば
					json = File.ReadAllText(filepath);
					list = JsonUtility.FromJson<MyUtility.BorouteInfoForSearching>(json).data;
				}else{
					list = new List<MyUtility.BorouteInformation>();
				}

				//リストに追加
				if (!isNew){
					//上書き保存
					int num = list.Count;
					for(int i = 0 ; i < num ; i++){
						if (list[i].timestamp.Equals(ts)){
							list[i] = eManager.GetBorouteInfo();
							break;
						}
					}
				}else{
					//新規作成
					list.Add(eManager.GetBorouteInfo());
				}
				MyUtility.BorouteInfoForSearching b = new MyUtility.BorouteInfoForSearching();
				b.data = new List<MyUtility.BorouteInformation>(list);
				json = JsonUtility.ToJson(b);
				//ファイルに書き込む
				File.WriteAllText(filepath, json);
			}

	    }catch (Exception e) {
	        Debug.Log("The process failed: " + e.ToString());
	        string p = Application.persistentDataPath + Observer.ROUTEPATH + ts;
	        if (string.IsNullOrEmpty(ts) && !Directory.Exists(p)){
				Directory.Delete(p, true);
			}
	    }
	}


	public void WriteThumbnail(string filepath){
		Texture2D texture = wallManager.GetMasterWallImage();
		Texture2D newTex = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
		newTex.LoadRawTextureData(texture.GetRawTextureData());
		int w = texture.width;
    	int h = texture.height;
    	//widhtとheightの大きいほうを選ぶ
    	int max = Mathf.Max(w, h);
    	//maxが200になるように縮小する
    	float rate = max / 200.0f;
    	TextureScale.Bilinear(newTex, (int)(w/rate), (int)(h/rate));
    	MyUtility.WriteImage(newTex, filepath);
	}


	public bool IsNew(){
		return isNew;
	}

	public void SaveTemporary(){
		SaveBoroute(true);
	}


	private void OnApplicationPause(bool pauseStatus){

		if (pauseStatus){
			//アプリ中断
			if (wallManager.IsWallImagePrepared()){
				//現在のボルートを一時フォルダに書き込む
				SaveTemporary();				
			}
		}else{
			//アプリ復帰
			//一時フォルダの削除
			DeleteTemp();
		}
	}

	private void DeleteTemp(){
		string direcPath = Application.persistentDataPath + EditorManager.BOROUTEPATH + "temp" ;
		try{
			//tempディレクトリが存在する場合
			if (Directory.Exists(direcPath)){
				string path = direcPath + "/Wall.png";
				if (File.Exists(path)){
					wallManager.CommitWallImage(MyUtility.LoadImage(path));
				}
            	//ディレクトリの削除
				Directory.Delete(direcPath, true);
			}
		}catch (Exception e){
			Debug.Log("The process failed: " + e.ToString());
		}
	}

	public void BorouteFromJson(string json){
		MyUtility.Boroute bor = JsonUtility.FromJson<MyUtility.Boroute>(json);
		wallManager.LoadMarks(bor.marks);
		hScenes.SetATList(bor.atList);
		hScenes.LoadMasterScenes(bor.masterScene);
		hScenes.LoadLatestAT();
		eManager.LoadBorouteInfo(bor.borouteInfo);
	}

	public string BorouteToJson(){
		MyUtility.Boroute bor = new MyUtility.Boroute();
		bor.borouteInfo = eManager.GetBorouteInfo();
		bor.atList = hScenes.GetATList();
		bor.masterScene = hScenes.GetMasterScenes();
		bor.marks = wallManager.GetMarks();

		return JsonUtility.ToJson(bor);
	}
}
