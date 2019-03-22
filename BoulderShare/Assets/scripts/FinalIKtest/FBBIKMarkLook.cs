using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBIKMarkLook : FBBIKMarkSP
{
	[SerializeField] private Transform poleTarget;
	[SerializeField] private Transform poleAxisFrom;
	[SerializeField] private Transform poleAxisTo;
	[SerializeField] private float length = 5.0f;

	public void Update(){
		poleTarget.position = (poleAxisTo.position - poleAxisFrom.position).normalized * length;
	}
}
