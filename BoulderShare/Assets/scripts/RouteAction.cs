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
	private bool isRouteTemp = false;
	private string timestamp;
	[SerializeField]
	private RouteView rv;
	[SerializeField]
	private Popup popup;

	[SerializeField]
	private Text place;
	[SerializeField]
	private Text grade;
	[SerializeField]
	private Text time;
	[SerializeField]
	private GameObject forTempBoroute;
	[SerializeField]
	private Image img;

	void Start(){
		GameObject obj = GameObject.Find("RouteView");
		rv = obj.GetComponent<RouteView>();
		popup = obj.transform.Find("Canvas_Popup").gameObject.GetComponent<Popup>();
	}

	//ボルート編集シーンに遷移
	public void LoadEditor(){
		//borouteselectedを生成
		GameObject obj = Instantiate(boroutePrefab);
		obj.name = "BoRouteSelected";
		Boroute b = obj.GetComponent<Boroute>();
		b.SetDRoute(dRouteJson);
		b.SetImg(img.sprite);
		b.SetDHolds(tempHoldsJson);
		b.SetDHScenes(tempHScenesJson);
		b.SetIsRouteTemp(isRouteTemp);
		SceneManager.LoadScene("edit2");
	}

	public void SetImg(Sprite spr){
		img.sprite = spr;
	}
	public void SetTimeText(string t){
		time.text = t;
	}

	public void SetPlaceText(string t){
		place.text = t;
	}

	public void SetGradeText(string t){
		grade.text = t;
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

	public void SetIsRouteTemp(bool b){
		isRouteTemp = b;
	}

	public void SetTimeStamp(string t){
		timestamp = t;
	}

	public Sprite GetImg(){
		return img.sprite;
	}
	public string GetTimeStamp(){
		return timestamp;
	}

	public void PushDeleteBtn(){
		string text = "本当に削除しますか？";
		popup.Open(DeleteRoute, null, text);
	}

	public void TreatBoRouteAsTemp(bool b){
		forTempBoroute.SetActive(b);
	}


	public void DeleteRoute(){
		if (isRouteTemp){
			rv.DeleteTemp();
		}else{
			rv.Delete(timestamp);
		}
		Destroy(gameObject);
	}
}