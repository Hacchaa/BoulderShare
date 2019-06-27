using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentDepth : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private ThreeDWall threeDWall;
    [SerializeField] private Transform root;
    [SerializeField] private CameraManager cManager;
    [SerializeField] private GameObject grid;
    [SerializeField] private float lookInAngle = 30.0f;
    private Vector3 startMovePos ;
    private Vector3 startRootPos;
    private Quaternion startRot;
    private float startFOV;
    private static int FINGER_NONE = -10;
    private static int finger = FINGER_NONE;
    private float weight = 0.01f;


    //イベント捕捉
    public void OnPointerDown(PointerEventData data){
    	if (finger == FINGER_NONE){
    		finger = data.pointerId;

	    	startRootPos = cManager.GetRootWorldPos();
	    	startMovePos = cManager.GetMovePos();
	    	startRot = cManager.Get3DRotation();
	    	startFOV = cManager.Get3DFOV();

	    	Quaternion rot = Quaternion.AngleAxis(lookInAngle,  root.rotation * Vector3.right) * root.rotation;
	   
	    	cManager.Transform3DWithAnim(root.position, rot, startFOV);

	    	grid.transform.position = root.position;
	    	grid.SetActive(true);
  		
  			threeDWall.HideTranslucentWall();
    	}
    }

    public void OnPointerUp(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;

			cManager.Transform3DWithAnim(root.position, startRot, startFOV, startMovePos);
			grid.SetActive(false);
		}
    }

	//マークを動かす
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			root.Translate(new Vector3(0.0f, 0.0f, data.delta.y * weight));
			grid.transform.position = root.position;
			cManager.SetRootWorldPos(root.position);

			threeDWall.ShowTranslucentWall();
		}
	}

}
