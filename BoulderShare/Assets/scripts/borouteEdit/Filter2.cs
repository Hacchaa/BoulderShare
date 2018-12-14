using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Filter2 : MonoBehaviour {
	[SerializeField]
	private Date_DD date_From ;
	[SerializeField]
	private Date_DD date_To ;
	[SerializeField]
	private InputField place;
	[SerializeField]
	private GradeList grade;
	[SerializeField]
	private RouteView2 rView;
	private int oldestDate;
	private int latestDate;

	public void Start(){
		//投稿されている中で最も古い日付
		oldestDate = rView.GetOldestDate();
		//投稿されている中で最も新しい日付
		latestDate = rView.GetLatestDate();

		//fromをoldestDateにする
		date_From.SetDate(oldestDate);
	}
	public void Open(){
		gameObject.SetActive(true);
	}

	public void Close(){
		gameObject.SetActive(false);
	}

	public void Cancel(){
		Close();
	}

	public void Apply(){
		bool b;
		string from = date_From.GetDate();
		string to = date_To.GetDate();

		if (oldestDate == -1 || latestDate == -1 || 
			int.Parse(from) <= oldestDate && int.Parse(to) >= latestDate){
			b = false;
		}else{
			b = true;
		}
		rView.Search(b, from, to, place.text, grade.GetGList());
		Close();
	}

	public void Clear(){
		date_From.SetDate(oldestDate);
		date_To.Init();
		place.text = "";
		grade.OffAll();
	}
}
