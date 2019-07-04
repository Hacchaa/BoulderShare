using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class RouteView2 : MonoBehaviour {
	private const string DONT_DESTROY_NAME = "InfoFromViewToEdit";
	private List<MyUtility.BorouteInformation> list;
	private int oldestDate;
	private int latestDate;
	private bool isTempExist = false;
	private GameObject tempOriginObj;
	private string tempTS;
	private GradeMap gm;

	[SerializeField]
	private GameObject routePrefab;
	[SerializeField]
	private GameObject root;
	[SerializeField]
	private ScrollRect routeSR;
	[SerializeField]
	private GameObject bInfoPrefab;

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
		string routePath =  Application.persistentDataPath + EditorManager.BOROUTEPATH + "search.json";
		string json = null;
		try{
			if(File.Exists(routePath)){
				json = File.ReadAllText(routePath);
			}
		}catch(Exception e) {
	        Debug.Log("The process failed: " + e.ToString());
		}

		list = new List<MyUtility.BorouteInformation>();

		//データがないとき
		if(String.IsNullOrEmpty(json)){
			return;
		}

		//読み込んだjsonをオブジェクトに変換してリストに変換する
		list = JsonUtility.FromJson<MyUtility.BorouteInfoForSearching>(json).data;
		Debug.Log("Count:"+list.Count);
	}

	public void ToEdit(){
		SceneManager.LoadScene("borouteEdit");
	}

	public void LoadRouteFirst(){
		string json = null;
		RouteViewContent raTemp = null;
		string path = Application.persistentDataPath + EditorManager.BOROUTEPATH;

		//borouteeditorに渡すオブジェクトの用意
		GameObject obj = Instantiate(bInfoPrefab);
		obj.name = DONT_DESTROY_NAME;
		InfoFromViewToEdit info = obj.GetComponent<InfoFromViewToEdit>();

		//場所の一覧をオブジェクトに格納する
		info.SetPlaceList(new List<string>(
			list
			.Where(x => !String.IsNullOrEmpty(x.place))
			.Select(x => x.place)
			.Distinct())
		);

		//一時保存ルートがある場合
		if (Directory.Exists(path + "temp/")){
			GameObject temp = Instantiate(routePrefab, root.transform);
			temp.SetActive(true);
			raTemp = temp.GetComponent<RouteViewContent>();

			isTempExist = true;
			//一時保存ボルートであることを伝える
			raTemp.TreatBoRouteAsTemp(true);

			if (File.Exists(path + "temp/boroute.json")){
				json = File.ReadAllText(path + "temp/boroute.json");
				MyUtility.Boroute bor = JsonUtility.FromJson<MyUtility.Boroute>(json);
				//ボルートをオブジェクトに表示
				raTemp.SetTimeStamp(bor.borouteInfo.timestamp);
				raTemp.SetTimeText(bor.borouteInfo.date);
				raTemp.SetPlaceText(bor.borouteInfo.place);
				raTemp.SetGradeText(bor.borouteInfo.grade + "");

				//画像を読み込む
				if (File.Exists(path + "temp/thumbnail.png")){
					raTemp.SetImg(MyUtility.CreateSprite(MyUtility.LoadImage(path + "temp/thumbnail.png")));
				}

				tempTS = bor.borouteInfo.timestamp;
			}
		}

		oldestDate = -1;
		latestDate = -1;

		//ルートの値をオブジェクトに代入
		foreach(MyUtility.BorouteInformation data in list){
			GameObject route = Instantiate(routePrefab, root.transform);
			route.SetActive(true);
			RouteViewContent ra = route.GetComponent<RouteViewContent>();

			//オブジェクトにデータを移す
			ra.SetTimeStamp(data.timestamp);
			ra.SetTimeText(data.date);
			ra.SetPlaceText(data.place);
			ra.SetGradeText(gm.GetGradeName(data.grade));

			//最も古い日を更新する
			int date = -1;
			bool ignoreDate = false;
			try{
				date = int.Parse(data.date);
			}catch(Exception e){
				Debug.Log("The process failed: " + e.ToString());
			}finally{
				if (date == -1){
					ignoreDate = true;
				}
			}
			if (!ignoreDate && (oldestDate == -1 || oldestDate > date)){
				oldestDate = date;
			}
			//最も新しい日を更新する
			if (!ignoreDate && (latestDate == -1 || latestDate < date)){
				latestDate = date;
			}

			//画像を読み込む
			path = Application.persistentDataPath + EditorManager.BOROUTEPATH + data.timestamp;
			if (File.Exists(path + "/thumbnail.png")){
				ra.SetImg(MyUtility.CreateSprite(MyUtility.LoadImage(path + "/thumbnail.png")));
			}
			//このボルートの一時保存がある場合
			if (!String.IsNullOrEmpty(tempTS) 
						&& tempTS.Equals(data.timestamp)){
				//一時保存元ボルートは表示しない
				route.SetActive(false);
				//一時保存が削除されたとき表示するために覚えておく
				tempOriginObj = route;
			}
		}
	}

	public void Search(bool isTermValid, string fromDate, string toDate, string place, List<int> gradeList){
		IEnumerable<MyUtility.BorouteInformation> fList = new List<MyUtility.BorouteInformation>(list);
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

	public void MakeRouteObjs(IEnumerable<MyUtility.BorouteInformation> fList){
		//temp以外のrootの子オブジェクトを削除
		//tempは必ず一番初めにくる
		bool flg = false;
		foreach (Transform child in root.transform ){
			if(!flg && isTempExist){
				flg = true;
			}else{
				GameObject.Destroy(child.gameObject);
			}
		}
		tempOriginObj = null;

		//ルートの値をオブジェクトに代入
		foreach(MyUtility.BorouteInformation data in fList){
			GameObject route = Instantiate(routePrefab, root.transform);
			route.SetActive(true);
			RouteViewContent ra = route.GetComponent<RouteViewContent>();

			//オブジェクトにデータを移す
			ra.SetTimeStamp(data.timestamp);
			ra.SetTimeText(data.date);
			ra.SetPlaceText(data.place);
			ra.SetGradeText(gm.GetGradeName(data.grade));

			//画像を読み込む
			string path = Application.persistentDataPath + EditorManager.BOROUTEPATH + data.timestamp;
			if (File.Exists(path + "/thumbnail.png")){
				ra.SetImg(MyUtility.CreateSprite(MyUtility.LoadImage(path + "/thumbnail.png")));
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
		string direcPath = Application.persistentDataPath + EditorManager.BOROUTEPATH + t ;
		try{
			//ディレクトリが存在する場合
			if (Directory.Exists(direcPath)){
            	//ディレクトリの削除
				Directory.Delete(direcPath, true);
			}
		}catch (Exception e){
			Debug.Log("The process failed: " + e.ToString());
		}

		if(t.Equals("temp")){
			//一時保存の元のボルートがある場合表示
			if (tempOriginObj != null){
				tempOriginObj.SetActive(true);
			}

			tempOriginObj = null;
			tempTS = "";
			isTempExist = false;

		}else{
			//listから指定されたルートを削除
			for(int i = 0 ; i < list.Count ; i++){
				if (list[i].timestamp.Equals(t)){
					list.RemoveAt(i);
					break;
				}
			}

			//listをjsonに変換
			MyUtility.BorouteInfoForSearching b = new MyUtility.BorouteInfoForSearching();
			b.data = new List<MyUtility.BorouteInformation>(list);
			string json = JsonUtility.ToJson(b);

			//新しいlistをファイルに保存
			string filePath = Application.persistentDataPath + EditorManager.BOROUTEPATH + "search.json";
			try{
				if (File.Exists(filePath)){
					File.WriteAllText(filePath, json);
				}
			}catch (Exception e){
				Debug.Log("The process failed: " + e.ToString());
			}
		}
	}
}
