using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class RouteView : MonoBehaviour {
	public GameObject routePrefab;
	public GameObject root;
	private List<DRoute.Data> list;
	private IEnumerable<string> placeList;
	private int oldestDate;
	private int latestDate;
	private bool isTempExist = false;
	[SerializeField]
	private ScrollRect routeSR;
	[SerializeField]
	private GameObject bInfoPrefab;
	private GameObject tempOriginObj;
	private string tempTS;
	private GradeMap gm;
	// Use this for initialization
	void Start () {
		gm = Resources.Load<GradeMap>("GradeMap");
		tempOriginObj = null;
		tempTS = "";
		oldestDate = -1;
		latestDate = -1;
		LoadData();
		LoadRouteFirst();
	}


	public int GetOldestDate(){
		return oldestDate;
	}

	public int GetLatestDate(){
		return latestDate;
	}

	private void LoadData(){
		//ルート情報をファイルから読み込む
		string routePath =  Application.persistentDataPath + Observer.ROUTEPATH + "route.json";
		string json = null;
		try{
			json = File.ReadAllText(routePath);
		}catch(Exception e) {
	        Debug.Log("The process failed: " + e.ToString());
		}

		list = new List<DRoute.Data>();

		//データがないとき
		if(String.IsNullOrEmpty(json)){
			return;
		}

		//読み込んだjsonをオブジェクトに変換してリストに変換する
		list.AddRange(DRoute.ConvertJsonToDRouteList(json).arr);

		//場所の一覧をオブジェクトに格納する
		placeList = list.Select(x => x.place);
		GameObject obj = Instantiate(bInfoPrefab);
		obj.name = "BoRouteInfo";
		obj.GetComponent<BoRouteInfo>().SetPlaceList(new List<string>(placeList));
	}

	public void ToEdit2(){
		SceneManager.LoadScene("edit2");
	}

	public void LoadRouteFirst(){
		string json = null;
		RouteAction raTemp = null;
		string path = Application.persistentDataPath + Observer.ROUTEPATH;
		//一時保存ルートがある場合
		if (Directory.Exists(path + "Temp/")){
			isTempExist = true;
			GameObject temp = Instantiate(routePrefab, root.transform);
			raTemp = temp.GetComponent<RouteAction>();

			//一時保存ボルートであることを伝える
			raTemp.SetIsRouteTemp(true);
			raTemp.TreatBoRouteAsTemp(true);
			if (File.Exists(path + "Temp/route.json")){
				json = File.ReadAllText(path + "Temp/route.json");
				DRoute.Data r = DRoute.ConvertJsonToDRoute(json);
				//ボルートをオブジェクトに表示
				raTemp.SetTimeStamp(r.timestamp);
				raTemp.SetDRoute(json);
				raTemp.SetTimeText(r.date);
				raTemp.SetPlaceText(r.place);
				raTemp.SetGradeText(r.grade + "");

			}
			json = null;
			if (File.Exists(path + "Temp/holds.json")){
				json = File.ReadAllText(path + "Temp/holds.json");
				raTemp.SetTempHoldsJson(json);
			}
			json = null;
			if (File.Exists(path + "Temp/hscenes.json")){
				json = File.ReadAllText(path + "Temp/hscenes.json");
				raTemp.SetTempHScenesJson(json);
			}
			json = null;
			//上書き編集かどうか(上書きはwallがここにない)
			if (File.Exists(path + "Temp/thumbnail.png")){
				//新規作成の場合
				raTemp.SetImg(LoadImage(path + "Temp/thumbnail.png"));
				/*
				//画像を移す
				string wallPath = Application.persistentDataPath + Observer.WALLPATH ;
				string fromPath = path + "Temp/Wall.png";

           	 	if (File.Exists(fromPath)){
            		File.Copy(fromPath, wallPath, true);
            	}*/
			}
		}

		oldestDate = -1;
		latestDate = -1;
		if (raTemp != null){
			tempTS = raTemp.GetTimeStamp();
		}
		//ルートの値をオブジェクトに代入
		foreach(DRoute.Data data in list){
			GameObject route = Instantiate(routePrefab, root.transform);
			RouteAction ra = route.GetComponent<RouteAction>();

			//オブジェクトにデータを移す
			ra.SetDRoute(DRoute.ConvertDRouteToJson(data));
			ra.SetTimeStamp(data.timestamp);
			ra.SetTimeText(data.date);
			ra.SetPlaceText(data.place);
			ra.SetGradeText(gm.GetGradeName(data.grade));

			//最も古い日を更新する
			int date = int.Parse(data.date);
			if (oldestDate == -1 || oldestDate > date){
				oldestDate = date;
			}
			//最も新しい日を更新する
			if (latestDate == -1 || latestDate < date){
				latestDate = date;
			}

			//画像を読み込む
			path = Application.persistentDataPath + Observer.ROUTEPATH + data.timestamp;
			if (File.Exists(path + "/thumbnail.png")){
				ra.SetImg(LoadImage(path + "/thumbnail.png"));
			}
			//このボルートの一時保存がある場合
			if (!String.IsNullOrEmpty(tempTS) 
						&& tempTS.Equals(data.timestamp)){
				//一時保存ボルートの画像を設定
				raTemp.SetImg(ra.GetImg());
				//一時保存元ボルートは表示しない
				route.SetActive(false);
				//一時保存が削除されたとき表示するために覚えておく
				tempOriginObj = route;
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

	public void Search(bool isTermValid, string fromDate, string toDate, string place, List<int> gradeList){
		IEnumerable<DRoute.Data> fList = new List<DRoute.Data>(list);
		int from, to;
		//日付でフィルター
		//fromとtoが逆の場合
		int fDTmp = int.Parse(fromDate);
		int tDTmp = int.Parse(toDate);

		//日付(yyyyMMdd)を整数に変換
		if (fDTmp > tDTmp){
			//fromとtoが逆の場合
			from = tDTmp;
			to = fDTmp;
		}else{
			//
			from = fDTmp;
			to = tDTmp;
		}

		if (isTermValid){
			fList = fList.Where(x => {int date = int.Parse(x.date);
							return date >= from && date <= to;});
		}

		if (!String.IsNullOrEmpty(place)){
			fList = fList.Where(x => x.place.Contains(place));
		}

		if (gradeList.Count > 0){
			fList = fList.Where(x => gradeList.Contains(x.grade));
		}

		MakeRouteObjs(fList);
	}

	public void MakeRouteObjs(IEnumerable<DRoute.Data> fList){
		//temp以外のrootの子オブジェクトを削除
		//tempは必ず一番初めにくる
		bool flg = false;
		foreach (Transform child in root.transform ){
			if(!flg && isTempExist){
				Debug.Log("tempisExsitpath");
				flg = true;
			}else{
				GameObject.Destroy(child.gameObject);
			}
		}
		tempOriginObj = null;

		//ルートの値をオブジェクトに代入
		foreach(DRoute.Data data in fList){
			GameObject route = Instantiate(routePrefab, root.transform);
			RouteAction ra = route.GetComponent<RouteAction>();

			//オブジェクトにデータを移す
			ra.SetDRoute(DRoute.ConvertDRouteToJson(data));
			ra.SetTimeStamp(data.timestamp);
			ra.SetTimeText(data.date);
			ra.SetPlaceText(data.place);
			ra.SetGradeText(gm.GetGradeName(data.grade));

			//画像を読み込む
			string path = Application.persistentDataPath + Observer.ROUTEPATH + data.timestamp;
			if (File.Exists(path + "/thumbnail.png")){
				ra.SetImg(LoadImage(path + "/thumbnail.png"));
			}
			//Debug.Log("tempTS:"+tempTS);
			//Debug.Log("timestamp:"+data.timestamp);
			//このボルートの一時保存がある場合
			if (!String.IsNullOrEmpty(tempTS) 
						&& tempTS.Equals(data.timestamp)){
				//一時保存元ボルートは表示しない
				route.SetActive(false);
				//一時保存が削除されたとき表示するために覚えておく
				tempOriginObj = route;
			}
		}

		//課題一覧を一番上にスクロールする
		routeSR.verticalNormalizedPosition = 1.0f;
	}

	//指定されたルートを削除する
	public void Delete(string t){
		//listから指定されたルートを削除
		for(int i = 0 ; i < list.Count ; i++){
			if (list[i].timestamp.Equals(t)){
				list.RemoveAt(i);
				break;
			}
		}

		//listをjsonに変換
		DRoute.DataArr dataArr = new DRoute.DataArr();
		dataArr.arr = list.ToArray();
		string json = DRoute.ConvertDRouteListToJson(dataArr);

		//新しいlistをファイルに保存
		string filePath = Application.persistentDataPath + Observer.ROUTEPATH + "route.json";
		try{
			if (File.Exists(filePath)){
				File.WriteAllText(filePath, json);
			}
		}catch (Exception e){
			Debug.Log("The process failed: " + e.ToString());
		}


		string direcPath = Application.persistentDataPath + Observer.ROUTEPATH + t ;
		try{
			//ディレクトリが存在する場合
			if (Directory.Exists(direcPath)){
            	//ディレクトリの削除
				Directory.Delete(direcPath, true);
			}
		}catch (Exception e){
			Debug.Log("The process failed: " + e.ToString());
		}
	}

	public void DeleteTemp(){
		string direcPath = Application.persistentDataPath + Observer.ROUTEPATH + "Temp" ;
		try{
			//tempディレクトリが存在する場合
			if (Directory.Exists(direcPath)){
            	//ディレクトリの削除
				Directory.Delete(direcPath, true);
			}
		}catch (Exception e){
			Debug.Log("The process failed: " + e.ToString());
		}
		//Debug.Log(tempOriginObj);
		//一時保存の元のボルートがある場合表示
		if (tempOriginObj != null){
			tempOriginObj.SetActive(true);
		}

		tempOriginObj = null;
		tempTS = "";
		isTempExist = false;

		
	}
}
