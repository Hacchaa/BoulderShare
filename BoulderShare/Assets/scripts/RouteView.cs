using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RouteView : MonoBehaviour {
	public GameObject routePrefab;
	public GameObject root;
	private List<DRoute.Data> list;

	// Use this for initialization
	void Start () {
		LoadRoute();
	}
	public void ToEdit2(){
		SceneManager.LoadScene("edit2");
	}
	public void LoadRoute(){
		string json = null;
		Image tempImg = null;
		string tempTStamp = "";
		string path = Application.persistentDataPath + Observer.ROUTEPATH;
		//一時保存ルートがある場合
		if (Directory.Exists(path + "Temp/")){
			GameObject temp = Instantiate(routePrefab, root.transform);
			tempImg = temp.transform.Find("Image").GetComponent<Image>();
			RouteAction ra = temp.GetComponent<RouteAction>();

			if (File.Exists(path + "Temp/route.json")){
				json = File.ReadAllText(path + "Temp/route.json");
				ra.SetDRoute(json);
				//タイムスタンプを記録しておく
				tempTStamp = DRoute.ConvertJsonToDRoute(json).timestamp;
			}
			json = null;
			if (File.Exists(path + "Temp/holds.json")){
				json = File.ReadAllText(path + "Temp/holds.json");
				ra.SetTempHoldsJson(json);
			}
			json = null;
			if (File.Exists(path + "Temp/hscenes.json")){
				json = File.ReadAllText(path + "Temp/hscenes.json");
				ra.SetTempHScenesJson(json);
			}
			json = null;
			//上書き編集かどうか(上書きはwallがここにない)
			if (File.Exists(path + "Temp/Wall.png")){
				//新規作成の場合
				tempImg.sprite = LoadImage(path + "Temp/Wall.png");
				//temptStampを初期化
				tempTStamp = "";
			}
		}
		//ルート情報をファイルから読み込む
		string routePath =  path + "route.json";
		try{
			json = File.ReadAllText(routePath);
		}catch(Exception e) {
	        Debug.Log("The process failed: " + e.ToString());
		}

		//データがないとき、何もしない
		if(String.IsNullOrEmpty(json)){
			return;
		}

		//読み込んだjsonをオブジェクトに変換してリストに変換する
		list = new List<DRoute.Data>();
		list.AddRange(DRoute.ConvertJsonToDRouteList(json).arr);

		//ルートの値をオブジェクトに代入
		foreach(DRoute.Data data in list){
			GameObject route = Instantiate(routePrefab, root.transform);
			GameObject routeInfo = route.transform.Find("RouteInfo").gameObject;
			Text time = routeInfo.transform.Find("Time").gameObject.GetComponent<Text>();
			Text place = routeInfo.transform.Find("Place").gameObject.GetComponent<Text>();
			Text grade = routeInfo.transform.Find("Grade").gameObject.GetComponent<Text>();
			Image img = route.transform.Find("Image").GetComponent<Image>();
			RouteAction ra = route.GetComponent<RouteAction>();

			//オブジェクトにデータを移す
			ra.SetDRoute(DRoute.ConvertDRouteToJson(data));
			time.text += data.date;
			place.text += data.place;
			grade.text += data.grade;

			//画像を読み込む
			path = Application.persistentDataPath + Observer.ROUTEPATH + data.timestamp;
			if (File.Exists(path + "/Wall.png")){
				if (!String.IsNullOrEmpty(tempTStamp) 
						&& tempTStamp.Equals(data.timestamp)){
					//一時保存ボルートの元のボルートの場合
					tempImg.sprite = LoadImage(path + "/Wall.png");
					//一時保存元ボルートは表示しない
					Destroy(route);
				}else{
					//一時保存ボルートでない
					img.sprite = LoadImage(path + "/Wall.png");
				}
			}
		}
	}

	private Sprite LoadImage(string path){
		Texture2D texture = new Texture2D(0, 0);
		texture.LoadImage(LoadBytes(path));
		return Sprite.Create(
			texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f),
            texture.height/4);
	}

	private byte[] LoadBytes(string path) {
		FileStream fs = new FileStream(path, FileMode.Open);
		BinaryReader bin = new BinaryReader(fs);
		byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
		bin.Close();
		return result;
	}
}
