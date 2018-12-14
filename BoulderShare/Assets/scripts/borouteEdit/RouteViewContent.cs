using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//routeViewシーンで一覧表示するオブジェクトのスクリプト
public class RouteViewContent : MonoBehaviour {
	private string timestamp;

	[SerializeField]
	private RouteView2 rv;
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

	//ボルート編集シーンに遷移
	public void LoadEditor(){
		GameObject obj = DontDestroyOnLoadManager.Get("InfoFromViewToEdit");
		InfoFromViewToEdit info = obj.GetComponent<InfoFromViewToEdit>();
		if (forTempBoroute.activeSelf){
			info.SetDirName("temp");
		}else{
			info.SetDirName(timestamp);
		}
		info.SetIsNew(false);
		SceneManager.LoadScene("borouteEdit");
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


	private void DeleteRoute(){
		if (forTempBoroute.activeSelf){
			rv.Delete("temp");
		}else{
			rv.Delete(timestamp);
		}
		Destroy(gameObject);
	}
}
