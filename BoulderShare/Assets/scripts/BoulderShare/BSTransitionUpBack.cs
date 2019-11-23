using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTransitionUpBack : BSTransitionBase
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
		float height = screenRoot.rect.height / 4.0f;

		screenRoot.localPosition = new Vector2(0.0f, height * t);
	}


	//##画面遷移を始める前に行う処理
	//
	//precondition:
	//postcondition:
	override public void Ready(bool isReverse){
		base.Ready(isReverse);

		if (!isReverse){
			screenRoot.localPosition = Vector2.zero;
		}else{
			screenRoot.localPosition = new Vector2(0.0f, screenRoot.rect.height / 4.0f);
		}
	}

	override public void Complete(bool isShowed){
		base.Complete(isShowed);
		//screenRoot.gameObject.SetActive(isReverse);
		//cg.blocksRaycasts = isReverse;
	}
}
