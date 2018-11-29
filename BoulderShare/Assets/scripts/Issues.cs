using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Issues : MonoBehaviour {
	[SerializeField]
	private ThreeDModel model;
	[SerializeField]
	private List<string> fList;
	[SerializeField]
	private GameObject failedList_ShowObj;
	[SerializeField]
	private GameObject failedTextPrefab;

	void Awake(){
		fList = new List<string>();
	}
	public void Open(){
		model.ChangeMode((int)ThreeDModel.Mode.DEFAULT);
		this.gameObject.SetActive(true);
	}

	public void Close(){
		model.ChangeMode((int)ThreeDModel.Mode.WINDOW);
		this.gameObject.SetActive(false);
	}

	//現在の原因リストを返す
	public List<string> GetList(){
		return new List<string>(fList);
	}

	//新しい原因リストに更新する
	public void Regist(List<string> list){
		//showObjの子オブジェクトを削除する
		foreach(Transform child in failedList_ShowObj.transform){
			GameObject.Destroy(child.gameObject);
		}

		//listをtextオブジェクトにしてshowObjの子に設定
		foreach(string str in list){
			GameObject t = Instantiate(failedTextPrefab, failedList_ShowObj.transform);
			t.GetComponent<Text>().text = str;
		}
		fList = new List<string>(list);
	}
}
