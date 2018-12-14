using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FailedListView: MonoBehaviour {
	[SerializeField]
	private List<string> fList;
	[SerializeField]
	private GameObject failedList_ShowObj;
	[SerializeField]
	private GameObject failedTextOrigin;
	[SerializeField]
	private HScenes2 hScenes;
	private bool isUpdateNeed ;
	void Awake(){
		isUpdateNeed = false;
		fList = new List<string>();
	}

	public void Open(){
		if (isUpdateNeed){
			Regist(hScenes.GetFailedList());
			isUpdateNeed = false;
		}
		this.gameObject.SetActive(true);
	}

	public void Close(){
		this.gameObject.SetActive(false);
	}

	public void SetIsUpdateNeed(bool b){
		isUpdateNeed = b;
	}

	//現在の原因リストを返す
	public List<string> GetList(){
		return new List<string>(fList);
	}

	public bool IsExist(){
		return isUpdateNeed || fList.Any();
	}

	//新しい原因リストに更新する
	private void Regist(List<string> list){
		//showObjの子オブジェクトを削除する
		foreach(Transform child in failedList_ShowObj.transform){
			GameObject.Destroy(child.gameObject);
		}

		//listをtextオブジェクトにしてshowObjの子に設定
		foreach(string str in list){
			GameObject t = Instantiate(failedTextOrigin, failedList_ShowObj.transform);
			t.GetComponent<Text>().text = str;
			t.SetActive(true);
		}
		fList = new List<string>(list);
	}
}
