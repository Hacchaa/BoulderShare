using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//routeViewシーンで一覧表示するオブジェクトのスクリプト
public class RouteAction : MonoBehaviour {
	public GameObject boroutePrefab;
	private string dRouteJson;
	private string tempHoldsJson;
	private string tempHScenesJson;
	//ボルート編集シーンに遷移
	public void LoadEditor(){
		//borouteselectedを生成
		GameObject obj = Instantiate(boroutePrefab);
		obj.name = "BoRouteSelected";
		Boroute b = obj.GetComponent<Boroute>();
		b.SetDRoute(dRouteJson);
		b.SetImg(gameObject.transform.Find("Image").GetComponent<Image>().sprite);
		SceneManager.LoadScene("edit2");
	}

	//ボルートデータ保持
	public void SetDRoute(string d){
		dRouteJson = d;
	}

	public void SetTempHoldsJson(string d){
		tempHoldsJson = d;
	}

	public void SetTempHScenesJson(string d){
		tempHScenesJson = d;
	}
}