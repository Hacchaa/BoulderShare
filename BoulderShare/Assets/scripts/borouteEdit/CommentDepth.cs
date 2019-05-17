using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentDepth : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private SceneComment3D sc;
    [SerializeField] private Transform root;
    [SerializeField] private CameraManager cManager;

    private static int FINGER_NONE = -10;
    private static int finger = FINGER_NONE;
    private float weight = 0.01f;


    //イベント捕捉
    public void OnPointerUp(PointerEventData data){
    }
    public void OnPointerDown(PointerEventData data){
    }

    public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;

		}
	}

	//マークを動かす
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			root.Translate(new Vector3(0.0f, 0.0f, data.delta.y * weight));
			cManager.SetRootWorldPos(root.position);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;
		}
	}

}
