using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//routeviewシーンでグレードでフィルターをかける為のUIのクラス
public class GradeList : MonoBehaviour {
	[SerializeField]
	private Toggle[] grade;

	//それぞれのgradeが選択されているかどうかを返す
	public List<int> GetGList(){
		List<int> list = new List<int>();

		for(int i = 0 ; i < grade.Length ; i++){
			if (grade[i].isOn){
				//gradeID = i;
				list.Add(i);
			}
		}

		return list;
	}

	public void OffAll(){
		for(int i = 0 ; i < grade.Length ; i++){
			grade[i].isOn = false;
		}
	}
}
