using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGData{
	private int id;

	public SGData(int i){
		id = i;
	}

	public int GetID(){
		return id;
	}

	public int CompareTo(SGData another){
		return another.GetID() - id;
	}
}
