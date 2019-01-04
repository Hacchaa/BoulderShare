using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentExtendWidth : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private SceneComment sc;
    [SerializeField]
    private Camera cam;

    private static int FINGER_NONE = -10;
    private static int finger = FINGER_NONE;
    private static float WIDTH_MIN = 40.0f;

    [SerializeField]
    private bool isLeft ;
    private float offsetDeg;

    public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;

			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z));

			p = sc.transform.InverseTransformPoint(p);
			offsetDeg = Vector2.Angle(
				p, 
				sc.transform.InverseTransformPoint(transform.position));

			if (p.y < 0){
				offsetDeg *= -1;
			}
			if (isLeft){
				offsetDeg *= -1;
			}

			//Debug.Log("offsetDeg"+offsetDeg);
		}
	}

	//マークを動かす
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z));

			Vector3 dir = p - sc.transform.position;
			float deg = Vector2.Angle(dir, Vector2.right);
			
			if (dir.y < 0){
				deg *= -1;
			}
			if (isLeft){
				deg += 180.0f;
			}
			sc.transform.rotation = Quaternion.Euler(0.0f, 0.0f, deg - offsetDeg);

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
