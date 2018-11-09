using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GradeMap : ScriptableObject {

	[SerializeField]
	private string[] gradeNames;

	public string GetGradeName(int index){
		if (gradeNames.Length <= index){
			return "";
		}
		return gradeNames[index];
	}

	public string[] GetGradeNames(){
		return gradeNames;
	}
}
