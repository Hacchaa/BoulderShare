using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BNodeData : IBNodeData
{
	private int id ;
	public BNodeData(){
		this.id = -1;
	}
	public BNodeData(int id){
		this.id = id;
	}

	public int GetID(){
		return id;
	}

	public int CompareTo(IBNodeData another){
		return another.GetID() - id;
	}
}
