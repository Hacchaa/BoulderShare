using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BorouteAndInformation : MonoBehaviour
{
	[SerializeField] private BorouteLSManager2 lsManager;
	[SerializeField] private MyUtility.Boroute boroute;
	private int lendingIndex;

	public void Init(MyUtility.Boroute b = null){
		if (b == null){
			boroute = new MyUtility.Boroute();
		}else{
			boroute = b;
		}
		lendingIndex = -1;
	}

	public void Write(){
		lsManager.SaveBoroute();
	}

	public bool IsLending(){
		return lendingIndex != -1;
	}

	public void ReleaseLending(){
		lendingIndex = -1;
	}

	public bool IsRecordEmpty(){
		return !boroute.records.Any();
	}

	public void SetIncline(int inc){
		boroute.borouteInfo.incline = inc;
	}

	public void SetHumanScale(float s){
		boroute.borouteInfo.scaleH2M = s;
	}

	public MyUtility.BorouteInformation GetInfo(){
		return boroute.borouteInfo;
	}

	public void SetInfo(MyUtility.BorouteInformation info){
		boroute.borouteInfo = info;
	}

	public MyUtility.Boroute GetBoroute(){
		return boroute;
	}

	public void AddRecord(List<MyUtility.AttemptTree> atList, List<MyUtility.Mark> m, List<MyUtility.Scene> scenes){
		MyUtility.ClimbRecord rec = new MyUtility.ClimbRecord();
		rec.attempts = atList;
		rec.marks = m;
		rec.masterScenes = scenes;
		boroute.records.Add(rec);
	}

	public List<MyUtility.ClimbRecord> GetRecords(){
		return new List<MyUtility.ClimbRecord>(boroute.records);
	}

	public MyUtility.ClimbRecord LendLatesteRecord(){
		if (IsRecordEmpty()){
			return null;
		}
		return boroute.records[boroute.records.Count-1];
	}

	public MyUtility.ClimbRecord LendRecord(int index){
		if (index < 0 || index > boroute.records.Count - 1){
			return null;
		}
		lendingIndex = index;

		return boroute.records[index];
	}

	public void UpdateLendingRecord(List<MyUtility.AttemptTree> atList, List<MyUtility.Mark> m, List<MyUtility.Scene> scenes){
		if (lendingIndex < 0){
			return ;
		}		

		MyUtility.ClimbRecord rec = new MyUtility.ClimbRecord();
		rec.attempts = atList;
		rec.marks = m;
		rec.masterScenes = scenes;

		boroute.records[lendingIndex] = rec;

		lendingIndex = -1;
	}
}
