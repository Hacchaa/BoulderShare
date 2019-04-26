using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GarbageFor2DMark : MonoBehaviour , IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private Transform garbageIcon;
	[SerializeField] private float moveDist = 20.0f;
	[SerializeField] private float animDuration = 0.1f;

	public void OnDrop(PointerEventData data){
		//Debug.Log("OnDrop");
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			GameObject obj = data.pointerDrag;
			twoDWallMarks.DeleteMark(obj.name);
			Destroy(obj);
			PointerExitAnimation();
		}
	}

	public void OnPointerEnter(PointerEventData data){
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			PointerEnterAnimation();
		}
	}

	public void OnPointerExit(PointerEventData data){
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			PointerExitAnimation();
		}
	}

	public void PointerEnterAnimation(){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(()=>
		{
			garbageIcon.localPosition = Vector3.zero;
		})
		.Append(garbageIcon.DOLocalMoveY(-moveDist, animDuration))
		.SetRelative();

		seq.Play();
	}

	public void PointerExitAnimation(){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(()=>
		{
			garbageIcon.localPosition = new Vector3(0.0f, -moveDist, 0.0f);
		})
		.Append(garbageIcon.DOLocalMoveY(moveDist, animDuration))
		.SetRelative()
		.OnComplete(()=>
		{
			garbageIcon.localPosition = Vector3.zero;
		});

		seq.Play();
	}
}
