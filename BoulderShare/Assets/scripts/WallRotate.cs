using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRotate : MonoBehaviour {
	[SerializeField]
	private Transform wallTrans;
	public float duration = 1.0f;
	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	private bool isClockWise;

	public void RotateVertically(bool isClockWise){
		this.isClockWise = isClockWise;
		StartCoroutine(WallRot());
	}

	private IEnumerator WallRot(){
		float startTime = Time.time;
		float moveRot = 90.0f;
		if (!isClockWise){
			moveRot *= -1;
		}

		float oldRot = 0.0f;

		while((Time.time - startTime) < duration){
			float curRot = moveRot * animCurve.Evaluate((Time.time - startTime) / duration);
			wallTrans.Rotate(0.0f, 0.0f, curRot - oldRot);
			oldRot = curRot;
			yield return null;
		}

		wallTrans.Rotate(0.0f, 0.0f, moveRot - oldRot);
	}
}
