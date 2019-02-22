using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIKHand : VRIKComponent
{
	[SerializeField] private List<float> xAngles;
	[SerializeField] private int xAngleIndex;
	[SerializeField] private List<float> yAngles;
	[SerializeField] private int yAngleIndex;
	[SerializeField] private List<float> zAngles;
	[SerializeField] private int zAngleIndex;
	private Vector3 handBaseAngles;

	public override void Init(){
		ResetPos();
		zAngleIndex = 0;
		xAngleIndex = 0;
		yAngleIndex = 0;

		handBaseAngles = avatar.localRotation.eulerAngles;
	}

	public override void Correct(){
		transform.position = avatar.position;
		avatar.localRotation = Quaternion.Euler(handBaseAngles + new Vector3(xAngles[xAngleIndex], yAngles[yAngleIndex], zAngles[zAngleIndex]));
	}
 
}
