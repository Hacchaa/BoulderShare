using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTransitionLeftFront : BSTransitionBase
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
		float width = screenRoot.rect.width;

		screenRoot.localPosition = new Vector2(width * (1.0f - t), 0.0f);
	}

	//##画面遷移を始める前に行う処理
	//
	//precondition:
	//postcondition:
	override public void Ready(bool isReverse){
		screenRoot.gameObject.SetActive(true);
		cg.blocksRaycasts = false;

		if (!isReverse){
			screenRoot.localPosition = new Vector2(screenRoot.rect.width, 0.0f);
		}else{
			screenRoot.localPosition = Vector2.zero;
		}
	}

	override public void Complete(bool isReverse){
		//screenRoot.gameObject.SetActive(!isReverse);
		//cg.blocksRaycasts = !isReverse;
	}
}
