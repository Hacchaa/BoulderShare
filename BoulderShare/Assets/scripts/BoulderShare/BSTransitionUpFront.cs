using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTransitionUpFront : BSTransitionBase
{

	//##画面遷移のはじめと終わりの間のui部品を線形補間する。
	//tは0より小さければ0として、1より大きければ1として扱う。
	//
	//param
	//float t:
	//
	//precondition:
	//postcondition:
	override public void TransitionLerp(float t){
		t = Mathf.Lerp(0.0f, 1.0f, t);
		float height = screenRoot.rect.height;

		screenRoot.localPosition = new Vector2(0.0f, -(height * (1.0f - t)));
	}


	//##画面遷移を始める前に行う処理
	//
	//precondition:
	//postcondition:
	override public void Ready(bool isReverse){
		base.Ready(isReverse);

		if (!isReverse){
			screenRoot.localPosition = new Vector2(0.0f, -screenRoot.rect.height);
		}else{
			screenRoot.localPosition = Vector2.zero;
		}
	}

	override public void Complete(bool isShowed){
		base.Complete(isShowed);
		//screenRoot.gameObject.SetActive(isReverse);
		//cg.blocksRaycasts = isReverse;
	}
}
