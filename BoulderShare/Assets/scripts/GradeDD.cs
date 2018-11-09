using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//edit2のpostにあるgrade初期化クラス
public class GradeDD : MonoBehaviour {
	[SerializeField]
	private Dropdown dd;

	// Use this for initialization
	void Start () {
		GradeMap gm = Resources.Load<GradeMap>("GradeMap");
		dd.ClearOptions();
		List<string> list = new List<string>();
		list.AddRange(gm.GetGradeNames());
		dd.AddOptions(list);
	}
}
