using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentExtendWidth3D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    private SceneComment3D sc;
    private Camera cam;
    private float baseDepth;

    private static int FINGER_NONE = -10;
    private static int finger = FINGER_NONE;
    private static float WIDTH_MIN = 40.0f;


    public void SetCamera(Camera camera){
    	cam = camera;
    }
    //イベント捕捉
    public void OnPointerUp(PointerEventData data){
    }
    public void OnPointerDown(PointerEventData data){
    }

    public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
		}
	}

	//マークを動かす
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));

			p = sc.transform.InverseTransformPoint(p);
        	float length = Mathf.Abs(p.x);

        	if (length < WIDTH_MIN){
        		length = WIDTH_MIN;
        	}

        	sc.UpdateWidth(length*2);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;
		}
	}

}
